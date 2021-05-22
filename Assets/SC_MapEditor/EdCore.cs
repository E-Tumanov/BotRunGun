using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


/// <summary>
/// 
/// EdCore 
/// Медиатор для редактора. Организует/координирует работу управляющих элементов редактора
/// mbCamera, mbCursor, mbToolPanel, mbSectionGen. Всё через интерфейсы 
/// 
/// </summary>
public class EdCore : MonoBehaviour
{
    //============= UI widgets ==================
    [SerializeField] Text textItemName;
    [SerializeField] Transform toolKitHUD;
    [SerializeField] Text textCurStage;
    [SerializeField] Text textCurStageSID;
    [SerializeField] Text stageTime;

    // Для быстрого редактирования, можно задать имя файла с уровнем.
    // Чтоб можно было в редакторе без ренейма работать с уровнями
    [Header ("CUSTOM FILE NAME")]
    [SerializeField] string stageFileName;
    bool isCustomFileName = false;


    /*

        Вот-вот. Это же просто DI/
        В юньке можно просто FindOjectByType<> и всё.

        Нужно ещё фабрика тулкитов

        И фабрика объектов (class Stage должен использовать внешнюю фабрику). Нужно их руками линковать

        И смотри, чтоб никаких сраных статиков!

    */

    IEdCursor di_Cursor;
    IEdCamera di_Camera;
    IToolPanel di_ToolPanel;
    IEdSectionGen di_SectionGen;
    EdTkFactory di_TkFactory;
    IItemFactory di_ItemFactory;
    StageGroupManager groupManager = new StageGroupManager ();

    // текущий тулкит/ какой предмет ставим
    IEdToolKit curToolKit;

    // уровень
    Stage currStage;

    // делитель сетки
    int gridSize = 4;

    private void Awake ()
    {
        SysCore.SetupFrameRateGlobal ();

        //  ROOT_COMP
        {
            di_Cursor = FindObjectOfType<EdCursor> ();
            di_Camera = FindObjectOfType<EdCamera> ();
            di_ToolPanel = FindObjectOfType<EdToolPanel> ();
            di_SectionGen = FindObjectOfType<EdSectionGen> ();
            di_TkFactory = FindObjectOfType<EdTkFactory> ();
            di_ItemFactory = FindObjectOfType<RBGame.Factory.ItemFactory> ();

            Assert.IsNotNull (di_Cursor);
            Assert.IsNotNull (di_Camera);
            Assert.IsNotNull (di_ToolPanel);
            Assert.IsNotNull (di_SectionGen);
            Assert.IsNotNull (di_TkFactory);
        }

        ConfDB.LoadConfigsAndSave ();

        //  Запомним, что в режиме редактирования        
        G.isEditMode = true;
        G.isRoundStarted = false;

        //  НЕ Пауза. Тогда будут все анимации видны
        G.SetPause (false);

#if (UNITY_EDITOR) // если редактор, то можно грузить любую карту
        isCustomFileName = stageFileName != "";
#endif
        //  грузим уровень для редактирования
        if (isCustomFileName)
            currStage = StageManager.Create (stageFileName + ".json", di_ItemFactory);
        else
            currStage = StageManager.Create (StageUserManager.currStageFileName, di_ItemFactory);

        // Глобально. Выставим имя файла уровня
        G.stageFileName = currStage.stageFileName;

        // Посчитаем группы
        groupManager.GroupList (currStage.RebuildCoinList ());
        currStage.OnItemAdd += x => 
        {
            if (x.itemType == MAP_ITEM_TYPE.COIN)
                groupManager.GroupList (currStage.RebuildCoinList ());
        };

        // HUD. номер уровня. 
        textCurStage.text = (StageUserManager.currLevel + 1).ToString ();

        // HUD. айдишник уровня.
        textCurStageSID.text = "SID: " + currStage.sid;

        di_Camera.SetCursor (di_Cursor);

        di_Cursor.SetPos (currStage.cursorPos);
        di_Cursor.onChangePos += OnCursorCangePos;
        di_Cursor.SetGridSize (gridSize);

        // Панель инструментов
        di_ToolPanel.changeToolKit += OnSelectTool;
    }


    //  ==========================================
    //  Обработчик клика по инструменту
    void OnSelectTool (ItemInfo tk)
    {
        //  Удалим текущий инструмент
        if (curToolKit != null)
            curToolKit.DestroyIt ();

        // Создать новый тулкит
        curToolKit = di_TkFactory.Create (tk.id, currStage);

        // Разместим курсор
        curToolKit.SetCursorPos (di_Cursor.GetSnapPos ());

        // добавим кнопки от нового тулкита
        curToolKit.GetVControl ().SetParent (toolKitHUD, false);

        // текст на кнопке (отображаемое имя тулкита)
        textItemName.text = ConfDB.item[tk.id].edname;
    }

    public void CMD_StageNext ()
    {
        var n = StageUserManager.currLevel;
        FilesTool.SaveFile (currStage.stageFileName, currStage.Export ().ToString ());
        StageUserManager.SelectNext ();
        if (n != StageUserManager.currLevel)
            SceneLoader.Editor ();
    }

    public void CMD_StagePrev ()
    {
        var n = StageUserManager.currLevel;
        FilesTool.SaveFile (currStage.stageFileName, currStage.Export ().ToString ());
        StageUserManager.SelectPrev ();
        if (n != StageUserManager.currLevel)
            SceneLoader.Editor ();
    }

    private void OnCursorCangePos (Vector3 pos)
    {
        di_SectionGen.SetTargetPos (pos);
        currStage.SetViewPoint (pos);
        curToolKit?.SetCursorPos (pos);

        if (isDeleteMode)
        {
            DelItem ();
        }

        //  setup HUD info
        {
            float crTime = pos.z / ConfDB.bot.BaseSpeed;
            float ttlTime = Mathf.Ceil (currStage.finishDist / (float)Stage.CHANK_SECTION_LEN) * Stage.CHANK_SECTION_LEN / ConfDB.bot.BaseSpeed;
            stageTime.text = "time: " + (crTime).ToString ("0.0") + ":" + ttlTime.ToString ("0.0");
            if (crTime > ttlTime)
            {
                stageTime.text = "Finished";
            }
            stageTime.text += "  z: " + (int)di_Cursor.GetRealPos ().z;
        }
    }


    bool isDeleteMode;
    public void CMD_ToggleDelMode (bool isDeleteMode)
    {
        this.isDeleteMode = isDeleteMode;
        if (this.isDeleteMode)
            DelItem ();
    }

    private void DelItem ()
    {
        if (currStage.IsPlaceBusy (di_Cursor.GetSnapPos ()))
        {
            currStage.DeleteItemOnPos (di_Cursor.GetSnapPos (), false);
            groupManager.GroupList (currStage.RebuildCoinList ());
        }
    }

    //  ===================================================
    //  Сохранить и запустить на тест, текущий уровень
    public void CMD_SaveStage ()
    {
        var export = currStage.Export ();
        FilesTool.SaveFile (currStage.stageFileName, export.ToString (3));

        NetCmd.UploadUserMap (currStage.sid, currStage.ver, export);

        // Запускаем текущую карту. Имя карты уже в G.
        SceneLoader.Game ();
    }

    public void CMD_MainMenu ()
    {
        SceneLoader.Menu ();
    }

    public void CMD_CopySection ()
    {
        currStage.CopySection (di_Cursor.GetSnapPos ());
    }

    public void CMD_PasteSection ()
    {
        currStage.PasteSection (di_Cursor.GetSnapPos ());
    }

    public void CMD_ClearSection ()
    {
        // todo
    }

    private void Update ()
    {
        var dd = new UpdateData ();
        foreach (var e in currStage.instancedItemList)
        {
            if (e.go != null)
                e.go.XUpdate (G.deltaTime, dd);
        }

#if (UNITY_EDITOR) || (UNITY_WIN)
        isDeleteMode = Input.GetKey (KeyCode.E); // erease
        if (isDeleteMode)
            DelItem ();
#endif
    }
}



/*
// Текущий зум
bool mIsZoom;
public void CMD_ToggleZoom()
{
    mIsZoom = !mIsZoom;
    if (mIsZoom)
        Camera.main.fieldOfView = 40;
    else
        Camera.main.fieldOfView = 60;
}


// Размер сетки
bool mIsSmallGrid;
public void CMD_ToggleGrid()
{
    mIsSmallGrid = !mIsSmallGrid;
    if (mIsSmallGrid)
        gridSize = 2; // 0.5 метра
    else
        gridSize = 4; // 1 метр
    mCursor.SetGridSize(gridSize);
}
*/
