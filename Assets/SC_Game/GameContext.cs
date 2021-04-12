using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RBGame.Factory;
using System;

namespace RBGame
{
    /// <summary>
    /// 
    /// ROOT COMPOSITION!
    /// 
    /// </summary>
    public class GameContext : GameContextBase
    {
        public static GameContext Instance;

        [HideInInspector] public GamePad GamePad;
        [HideInInspector] public RoadManager RoadManager;
        [HideInInspector] public BattleManager BattleManager;
        [HideInInspector] public BallsManager BallsManager;
        [HideInInspector] public CoinManager CoinManager;
        [HideInInspector] public TripManager TripManager;
        [HideInInspector] public Stage CurrStage;
        [HideInInspector] public IPlayerModel PlayerModel;
        [HideInInspector] public BossInfo BossInfo;
        [HideInInspector] public BossFactory BossFactory;
        [HideInInspector] public IItemFactory ItemFactory;
        [HideInInspector] public StageGroupManager GroupManager;

        public CfgRoad RoadConfig;

        public Color32 GetColorByNum (int colorNum)
        {
            if (colorNum < 0 || colorNum >= RoadConfig.ColorStack.Count)
                return Color.white;

            return RoadConfig.ColorStack[colorNum];
        }


        [SerializeField] string roadName;

        private void Awake ()
        {
            Instance = this;

            ConfDB.LoadConfigsAndSave ();
            //G.RoadConfig = RoadConfig;

            if (roadName != null && roadName != "")
                G.stageFileName = roadName + ".json";

            if (G.stageFileName == string.Empty)
                G.stageFileName = "101.json";

            
            GamePad = FindObjectOfType<GamePad> ();

            {
                ItemFactory = FindObjectOfType<ItemFactory> ();
                CurrStage = StageManager.Create (G.stageFileName, ItemFactory);
                GroupManager = new StageGroupManager ();
                GroupManager.GroupList (CurrStage.RebuildCoinList ());
            }

            BallsManager = GetComponent<BallsManager> ();
            CoinManager = GetComponent<CoinManager> ();
            TripManager = GetComponent<TripManager> ();
            RoadManager = FindObjectOfType<RoadManager> ();
            PlayerModel = GetComponent<PlayerModel> ();
            BattleManager = GetComponent<BattleManager> ();
            BossFactory = FindObjectOfType<BossFactory> ();

            BossInfo = CurrStage.bossInfo;
        }

        public float GetUpOffset (float zpos)
        {
            float mul = 1 - Mathf.Clamp01 ((TripManager.FinishDist - zpos) / 200.0f);
            return Mathf.Lerp (RoadConfig.GetUpOffset (zpos), 0, mul);
        }

        private void Update ()
        {
            Command.RunList ();
        }

        private void OnDestroy ()
        {
            //Instance = null;
        }
    }


    /// <summary>
    /// Игровые системы/объекты которым нужен GameContext(DI) наследуют это
    /// 
    /// Вообще, конечно, нужно убрать эти свойства и заставить себя(хихи)
    /// использовать префикс(будто) "di."
    /// т.е где-то в коде будет di.eve.OnBossKilled.AddListener()
    /// тогда всегда можно посмотреть какие зависимости у класса
    /// 
    /// а это сделано чтоб крайо выпилить было легко
    /// </summary>
    public class GModel : MonoBehaviour
    {
        protected GameContext di => GameContext.Instance;

        protected GameEventSystem eve => di.eve;
        protected IPlayerModel bot => di.PlayerModel;
        protected BossInfo bossInfo => di.BossInfo;
        protected BallsManager ballsManager => di.BallsManager;
        protected RoadManager roadMgr => di.RoadManager;
        protected TripManager trip => di.TripManager;
        protected CoinManager coin => di.CoinManager;
        protected BattleManager battleMgr => di.BattleManager;
        protected GamePad gamepad => di.GamePad;
        protected BossFactory factoryBoss => di.BossFactory;
        protected Stage stage => di.CurrStage;

        protected CfgRoad roadConfig => di.RoadConfig;
    }
}