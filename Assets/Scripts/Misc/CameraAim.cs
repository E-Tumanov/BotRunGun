using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/*
==========================================
Aim constrain
==========================================
*/
public enum CAMERA_STATE
{ 
    NONE,
    LOBBY,
    RUN,
    FIGHT,
    GAME_OVER, // fail, dance
    __end
}

namespace RBGame
{

    [ExecuteInEditMode]
    public class CameraAim : GModel
    {
        [SerializeField] Transform locator;

        [SerializeField] Vector3 startPos;
        [SerializeField] Vector3 startRot;

        [SerializeField] Vector3 startPosB;
        [SerializeField] Vector3 startPosFight;
        [SerializeField] Vector3 startRotFight;

        [SerializeField] float XFollowWeight;
        [SerializeField] float XFollowRad = 3;
        [SerializeField] float ZFollowWeight;
        [SerializeField] float currZFollowWeight;

        Vector3 interLocator;

        [Range (0, 1)]
        [SerializeField] float mlerpGameOver;

        [Range (0, 1)]
        [SerializeField] float mlerpFight;

        [SerializeField] CAMERA_STATE camState = CAMERA_STATE.LOBBY;


        Vector3 prevPos;
        Quaternion prevRot;

        void Start ()
        {
            interLocator = locator.position;

            Eve.OnGameOver.AddListener (this, x => {
                camState = CAMERA_STATE.GAME_OVER;
                
                prevPos = transform.position;
                prevRot = transform.rotation;

                MTask.Run (0, 3f, t => {
                    mlerpGameOver = t * t;
                });
            });

            Eve.OnPlayerFinished.AddListener (this, x => {
                camState = CAMERA_STATE.FIGHT;
                MTask.Run (0, 1f, t => {
                    mlerpFight = Mathf.Clamp01 (1.1f * t * t);
                });
            });
        }

        void LateUpdate ()
        {
            if (locator == null)
                return;

            float dd = Math.Abs (interLocator.x - locator.position.x) / XFollowRad;
            if (dd < 0.05f)
                dd = 0;

            interLocator.x = Mathf.Lerp (interLocator.x, locator.position.x, dd * XFollowWeight * G.deltaTime);
            interLocator.z = Mathf.Lerp (interLocator.z, locator.position.z, ZFollowWeight * 50 * G.deltaTime);

            Vector3 dstPosA = Vector3.Lerp (transform.position, interLocator - startPos, 0.5f);
            Quaternion dstRotA = Quaternion.Euler (startRot.x, startRot.y, startRot.z);

            if (camState == CAMERA_STATE.GAME_OVER)
            {
                Vector3 dstPos = interLocator - startPosB;
                Quaternion dstRot = Quaternion.LookRotation (interLocator - transform.position); // aim2 player
                transform.position = 0.5f * (transform.position + Vector3.Lerp (prevPos, dstPos, mlerpGameOver));
                transform.rotation = Quaternion.Lerp (prevRot, dstRot, Mathf.Clamp01 (2 * mlerpGameOver));
            }
            else if (camState == CAMERA_STATE.FIGHT)
            {
                Vector3 dstPos = interLocator - startPosFight;
                dstPos.x = 0;
                Quaternion dstRot = Quaternion.Euler (startRotFight.x, startRotFight.y, startRotFight.z);//not aim2 player
                transform.position = Vector3.Lerp (dstPosA, dstPos, mlerpFight);
                transform.rotation = Quaternion.Lerp (dstRotA, dstRot, Mathf.Clamp01 (mlerpFight));
            }
            else
            {
                transform.position = dstPosA;
                transform.rotation = dstRotA;
            }
        }
    }
}