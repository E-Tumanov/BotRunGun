using DG.Tweening;
using System.Collections;
using UIAnimatorCore;
using UIAnimatorDemo;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// Туторные кнопки. Затухание
/// 
/// </summary>
namespace RBGame
{
    public class Wdg_TutorButton : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvas;

        private void Start()
        {
            if (!G.isTutorEnable)
            {
                canvas.alpha = 0;
            }
            else
            {
                // N секунд до исчезновения кнопок
                canvas.alpha = 1;
                DOTween.Sequence().SetLink(gameObject).AppendInterval(5).onComplete = () =>
                {
                    canvas.DOFade(0, 3);
                };
            }
        }
    }
}
