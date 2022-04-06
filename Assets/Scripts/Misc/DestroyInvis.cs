using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RBGame
{
    /// <summary>
    /// Уничтожает объекты за спиной игрока
    /// </summary>
    public class DestroyInvis : GModel
    {
        public int dist = 40;

        void Update()
        {
            if (bot.position.z - transform.position.z > dist)
            {
                Destroy(gameObject);
            }
        }
    }
}