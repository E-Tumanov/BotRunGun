using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using TMPro;
using UIAnimatorCore;

public class ItemHMover : Item
{
    [Header("Если не 0, то считает только радиус")]
    public float extRad = 0;
    public Transform stolb;

    public Transform prefabDescFloor;

    Vector3 mStartPivotPos;
    Transform mPivot;
    Transform mDescFloor;

    float mDir = 0;
    float mCurPoint;
    float mMinX = -3.0f;
    float mMaxX = +3.0f;
    float mPeriodSec;

    int mPeriodTick;
    int mCurrPeriodTick;
    float yOffset;

    public override Vector3 pos => mStartPivotPos;
    public override bool IsUpdated() => true;

    private void Start()
    {
        stolb.localRotation = Quaternion.Euler(0, Random.value * 360, 0);
    }

    public override void Init(JSONNode param)
    {
        mStartPivotPos = new Vector3(param[0].AsFloat, 0, param[1].AsFloat);

        // начальное положение со смещением
        mCurPoint = mStartPivotPos.x;

        // стартовое направление и скорость
        mDir = 3 * param[2].AsFloat;

        if (!G.isRoundStarted) // создан, но раунд не запущен - значит редактор
        {
            /*
            mPivot = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            mPivot.localScale = Vector3.one * 0.3f;
            mPivot.transform.position = mStartPivotPos;
            */
            mDescFloor = Instantiate(prefabDescFloor);
            mDescFloor.position = mStartPivotPos + Vector3.up * 0.1f;

            string ss = "L";
            if (mDir > 0)
                ss = "R";
            ss += Mathf.Abs(mDir / 2).ToString(); // при симуляции mDir меняется
            mDescFloor.GetComponent<TextMeshPro>().text = ss;
        }

        // расчитаем время на полный период 
        float dist = 2 * (mMaxX - mMinX);
        mPeriodSec = dist / Mathf.Abs(mDir);

        mPeriodTick = mCurrPeriodTick = Mathf.FloorToInt(mPeriodSec / GSV.DTIME);
        // есть задумка, снепить скорость так, чтоб период был кратен GSV.DTIME

        // Отбросим целое кол-во периодов / прогоним симуляцию.
        {
            int simTime = G.tick % mPeriodTick;
            for (int i = 0; i < simTime; i++)
                SolveDstPoint();
        }

        yOffset = G.isRoundStarted ? cx.GetUpOffset(mStartPivotPos.z) : 0;
        transform.position = new Vector3(mCurPoint, yOffset, mStartPivotPos.z);
    }

    void SolveDstPoint()
    {
        mCurPoint += mDir * GSV.DTIME;

        if (mDir < 0)
        {
            if (mCurPoint < mMinX)
            {
                mCurPoint = mMinX;
                mDir *= -1;
            }
        }
        else
        {
            if (mCurPoint > mMaxX)
            {
                mCurPoint = mMaxX;
                mDir *= -1;
            }
        }

        /*
        mCurrPeriodTick--;
        if (mCurrPeriodTick == 0)
        {
            mCurrPeriodTick = mPeriodTick;
            mCurPoint = mStartPivotPos.x;
        }*/
    }

    private void OnDestroy()
    {
        if (mPivot != null)
            Destroy(mPivot.gameObject);
        if (mDescFloor != null)
            Destroy(mDescFloor.gameObject);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Player"))
        {
            cx.TripManager.CollisionItemTest (this);
        }
    }

    public override bool XCollision(UpdateData data)
    {
        return false;
        /*
        if (Mathf.Abs(transform.position.z - data.playerPos.y) > 2)
            return false;

        bool isCollison = (new Vector2(transform.position.x, transform.position.z) - data.playerPos).magnitude < (extRad + data.playerRad);
        return isCollison;
        */
    }


    public override void XUpdate(float dt, UpdateData data)
    {
        stolb.Rotate(0, 90 * Mathf.Abs(mDir) * dt, 0);
        SolveDstPoint();
        
        transform.position = new Vector3(mCurPoint, yOffset, mStartPivotPos.z);
    }
}