using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RBGame
{
    /// <summary>
    /// Выстрел
    /// </summary>
    public class FireBolt : MonoBehaviour
    {
        public void Init(Vector3 start, Vector3 end)
        {
            transform.position = start;
            transform.DOMove(end, 0.2f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}