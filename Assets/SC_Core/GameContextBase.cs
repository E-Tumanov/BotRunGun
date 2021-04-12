using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBGame
{
    /// <summary>
    /// Это вынес отдельно. Должно быть одинаковым для всех проектов
    /// </summary>
    public class GameContextBase : MonoBehaviour
    {
        GameEventSystem _eve;
        public GameEventSystem eve // rename 2 Eve
        {
            get
            {
                if (_eve == null)
                    _eve = new GameEventSystem();
                return _eve;
            }
        }

        GameCommandSystem _cmd;
        public GameCommandSystem Command
        {
            get
            {
                if (_cmd == null)
                    _cmd = new GameCommandSystem();
                return _cmd;
            }
        }
    }
}