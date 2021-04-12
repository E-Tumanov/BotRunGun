using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace RBGame
{
    /// <summary>
    /// Подсчет патронов
    /// </summary>
    public class BallsManager : GModel
    {
        /// <summary>
        /// color_num :int
        /// stack_size :int
        /// </summary>
        public event System.Action<StageGroupInfo> OnAddGroup = delegate { };
        public event System.Action<StageGroupInfo> OnDelGroup = delegate { };
        public int GroupCount => di.GroupManager.GroupCount; // DEPRECATED сколько групп
        public int TotalCount => di.GroupManager.TotalCoins;// сколько всего шаров на уровне
        public int CurrCount;// сколько сейчас осталось
        Stack<StageGroupInfo> colorStack = new Stack<StageGroupInfo> ();


        private void Start ()
        {
            di.GroupManager.OnGroupCollected += OnGroupCollected;
        }


        /// <summary>
        /// Забрать шар
        /// </summary>
        /// <returns></returns>
        public int PopTopBall ()
        {
            if (colorStack.Count != 0)
            {
                var grp = colorStack.Peek ();
                grp.CurrCount--;
                CurrCount--;
                Assert.IsTrue (grp.CurrCount >= 0);
                Assert.IsTrue (CurrCount >= 0);

                if (grp.CurrCount == 0)
                {
                    var dg = colorStack.Pop ();
                    OnDelGroup?.Invoke (dg);
                }

                return grp.GroupNum % 4;
            }
            return -1;
        }


        /// <summary>
        /// Какой шар на топе
        /// </summary>
        /// <returns></returns>
        public int PeekTopBallColor ()
        {
            if (colorStack.Count == 0)
                return -1;
            return colorStack.Peek ().GroupNum % 4;
        }


        /// <summary>
        /// Обработчик. Собрали группу
        /// </summary>
        private void OnGroupCollected (StageGroupInfo grp)
        {
            OnAddGroup (grp);
            colorStack.Push (grp);

            CurrCount += grp.MaxCount;

            // убрать отсюда в саунд манагера
            SoundManager.get_full_stack.Play ();
        }
    }
}