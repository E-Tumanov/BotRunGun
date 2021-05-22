using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    /// <summary>
    /// Плашка. "ПОБЕДА"
    /// </summary>
    public class Win_Victory : MonoBehaviour, IWindow
    {
        [SerializeField] Button nextBtn;

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }

        void Start()
        {
            nextBtn.onClick.AddListener(() => 
            {
                /* 
                 *  Не очень хорошо, когда вот так. Лучше бы GameUIManager подписался на нажатие
                 *  А то не видно, что сцену меняет. Да и второе окно Win_Defeat 
                 * */
                SceneLoader.Menu();
            });
        }
    }
}