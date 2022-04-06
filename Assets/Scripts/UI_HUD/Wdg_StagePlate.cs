using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Виджет. Плашка выбора уровня
/// 
/// </summary>
namespace RBGame
{
    public interface IStagePlate
    {
        void SetNumber(int num);
        void SetEnable(bool isEnable);
        void SetStarScore(int count);
        void SetCoinScore(int count);
        void SetRunCount(int count);
        void SetListener(System.Action onClick);
        void SelectIt();
        void UnselectIt();
    }


    public class Wdg_StagePlate : MonoBehaviour, IStagePlate
    {
        [SerializeField] Transform selectBorder;
        [SerializeField] Transform[] starList;
        [SerializeField] Text coinText;
        [SerializeField] Text runCountText;
        [SerializeField] Text mapNumber;
        
        void Start()
        {
            UnselectIt();
        }

        void ViewSelected(bool isSelected)
        {
            selectBorder.gameObject.SetActive(isSelected);
        }

        public void SetEnable(bool isEnable)
        {
            if (!isEnable)
            {
                mapNumber.color = Color.grey;
                GetComponent<Button>().interactable = false;
            }
        }
        
        public void SetNumber(int num)
        {
            mapNumber.text = num.ToString();
        }

        public void UnselectIt()
        {
            ViewSelected(false);
        }

        public void SelectIt()
        {
            ViewSelected(true);
        }

        public void SetRunCount(int count)
        {
            runCountText.text = count.ToString();
        }


        public void SetStarScore(int count)
        {
            if (count > 3)
                count = 3;
            for (int i = 0; i < 3; i++)
            {
                if (i < count)
                    starList[i].gameObject.SetActive(true);
                else
                    starList[i].gameObject.SetActive(false);
            }
        }

        public void SetCoinScore(int count)
        {
            coinText.text = count.ToString();
        }

        public void SetListener(System.Action onClick)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                onClick();
            });
        }
    }
}