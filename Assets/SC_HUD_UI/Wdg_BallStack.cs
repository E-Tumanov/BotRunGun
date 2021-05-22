using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace RBGame
{
    public class Wdg_BallStack : GModel
    {
        [SerializeField] List<Transform> ball;
        [SerializeField] AnimationCurve cvPopUp;
        [SerializeField] Image currBallShot;
        int prevColorNum = -1;

        void Start ()
        {
            currBallShot.gameObject.SetActive (false);

            di.BallsManager.OnAddGroup += BallsManager_OnAddGroup;
            di.BallsManager.OnDelGroup += BallsManager_OnDelGroup;
            Eve.OnPlayerFinished.AddListener (this, x => UpdateShotBall ());

            Eve.OnPlayerShot.AddListener (this, x => {
                UpdateShotBall ();
            });

            Eve.OnGameOver.AddListener (this, x => {
                gameObject.SetActive (false);
            });
        }


        // Смена цвета шара
        void UpdateShotBall ()
        {
            int colorNum = di.BallsManager.PeekTopBallColor ();
            if (colorNum < 0)
            {
                currBallShot.gameObject.SetActive (false);
                return;
            }
            currBallShot.gameObject.SetActive (true);
            currBallShot.sprite = ball[colorNum].GetComponent<Image> ().sprite;
            MTask.Run (currBallShot, 0, 0.25f, t => {
                currBallShot.transform.localScale = Vector3.one * cvPopUp.Evaluate (t);
            });
        }

        void BallsManager_OnDelGroup (StageGroupInfo _)
        {
            var n = transform.childCount;
            if (n != 0)
                Destroy (transform.GetChild (n - 1).gameObject);

            UpdateShotBall ();
        }

        // добавилась группа
        private void BallsManager_OnAddGroup (StageGroupInfo grp)
        {
            var tr = Instantiate (ball[grp.GroupNum % 4], transform, false);
            tr.gameObject.SetActive (true);
            MTask.Run (tr, 0, 0.5f, t => {
                tr.localScale = cvPopUp.Evaluate (t) * Vector3.one * 2;// * (1 + grp.MaxCount / 6.0f);
            });
        }
    }
}