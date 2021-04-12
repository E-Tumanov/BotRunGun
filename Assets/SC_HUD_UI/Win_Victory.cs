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
                SceneLoader.Menu();
            });
        }
    }
}