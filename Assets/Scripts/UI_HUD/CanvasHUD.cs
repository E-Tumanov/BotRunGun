using UnityEngine;

namespace RBGame
{
    /// <summary>
    ///  CanvasHUD
    ///  
    ///     Должно висеть на MainCanvas в каждой сцене
    /// - Даёт ссылку (static) на главный Canvas в сцене для UI_Manager
    /// - Отслеживает смену сцены, чтоб очистить UI_Manager
    /// - Открывает выбранное окно(openOnStartScene) при старте сцены
    /// </summary>
    public class CanvasHUD : MonoBehaviour
    {
        [SerializeField] WINDOW openOnStartScene = WINDOW.__end;

        private void Start()
        {
            UIManager.OpenWin(openOnStartScene, null);
        }
    }
}