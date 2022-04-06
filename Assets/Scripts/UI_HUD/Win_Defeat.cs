using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RBGame
{

    /// <summary>
    /// Надпись "Поражение"
    /// </summary>
    public class Win_Defeat : MonoBehaviour, IWindow
    {
        [SerializeField] Button restart;
        [SerializeField] Button next;

        void Start()
        {
            restart.onClick.AddListener(() => 
            {
                if (G.isEditMode)
                {
                    SceneLoader.Editor();
                }
                else
                {
                    SceneLoader.Menu();
                }
            });
        }

        public void DestroyWindow()
        {
            Destroy(gameObject);
        }
    }
}
