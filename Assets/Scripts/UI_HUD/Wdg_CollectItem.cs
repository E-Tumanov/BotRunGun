using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RBGame
{
    public class Wdg_CollectItem : MonoBehaviour
    {
        [SerializeField] Transform itemIcon;
        [SerializeField] Vector3 goalPos;

        [SerializeField] RectTransform start;
        [SerializeField] RectTransform goal;
        
        RectTransform pr;

        Vector2 PN (Vector3 refPos)
        {
            return 0.5f * new Vector2 (1 + refPos.x / 200.0f, 1 + refPos.y / 400.0f);
        }
        
        private void Start ()
        {
            var pr = transform.parent.GetComponent<RectTransform> ();
        }

        public void Emmit (System.Action onDoneMoveCoin)
        {
            var go = Instantiate (itemIcon, transform.parent, false).GetComponent<RectTransform> ();
            var spos = start.position + new Vector3 (Random.Range (-150, 150), Random.Range (-100, 100));
            var tscale = 0.5f + 0.5f * Random.value;
            MTask.Run (go, 0, 0.75f, t =>
            {
                var mt = Mathf.Pow (t, 0.25f);
                go.position = Vector2.Lerp (start.position, spos, mt);
                go.localScale = Vector3.one * tscale * mt;
            }, () =>
            {
                var endPos = goal.position;
                MTask.Run (go, 0, 0.75f, t =>
                {
                    go.position = Vector2.Lerp (spos, endPos, Mathf.Sqrt (t));
                },
                () =>
                {
                    onDoneMoveCoin ();
                    Destroy (go.gameObject);
                });
            });
        }
    }
}