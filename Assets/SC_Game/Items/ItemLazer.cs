using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

/// <summary>
/// 2 тумбы и лазер между ними
/// </summary>
public interface ILazer
{
    void SetAPos(Vector3 pos);
    void SetBPos(Vector3 pos);
    void Destroy();
}

/// <summary>
/// 
/// </summary>
public class ItemLazer : Item, ILazer
{
    [SerializeField] Transform p1;
    [SerializeField] Transform p2;
    [SerializeField] float tumbaCollisionRad = 0.4f;

    Vector3 mStartPivotPos;

    Vector3 mDir;
    float mLength;
    bool mSinglePlace;

    public override void Init(JSONNode param)
    {
        /*
        float ypos = 0;
        if (G.isRoundStarted)
            ypos = G.RoadConfig.GetUpOffset (param[1].AsFloat);

        mStartPivotPos = new Vector3(param[0].AsFloat, ypos, param[1].AsFloat);
        p1.position = mStartPivotPos;
        p2.position = mStartPivotPos + new Vector3(param[2].AsFloat, 0, param[3].AsFloat);
        */
        float ypos1 = 0;
        float ypos2 = 0;
        if (G.isRoundStarted)
        {
            ypos1 = di.GetUpOffset (param[1].AsFloat);
            ypos2 = di.GetUpOffset (param[1].AsFloat + param[3].AsFloat);
        }

        mStartPivotPos = new Vector3 (param[0].AsFloat, ypos1, param[1].AsFloat);
        p1.position = mStartPivotPos;
        p2.position = new Vector3 (mStartPivotPos.x + param[2].AsFloat, ypos2, mStartPivotPos.z + param[3].AsFloat);
        SolveData ();
    }

    void SolveData()
    {
        if ((p1.transform.position - p2.transform.position).magnitude > 0.25f)
        {
            mDir = p2.position - p1.position;
            mDir.y = 0;
            mLength = mDir.magnitude;
            mDir /= mLength;
        }
        else
        {
            mSinglePlace = true;
            mLength = 0.001f;
        }
    }

    public void SetAPos(Vector3 pos)
    {
        p1.position = pos;
        SolveData();
    }

    public void SetBPos(Vector3 pos)
    {
        p2.position = pos;
        SolveData();
    }

    public override Vector3 pos => mStartPivotPos;

    public override bool XCollision(UpdateData data)
    {
        //  не доехали даже до минимальной тумбы
        if (transform.position.z - data.playerPos.y > 1)
            return false;

        var posA = new Vector3 (p1.position.x, 0, p1.position.z);
        var posB = new Vector3 (p2.position.x, 0, p2.position.z);

        // ТУДУ: проверка если тумбы в одном месте

        Vector3 ppos = new Vector3(data.playerPos.x, 0, data.playerPos.y);
        float prad = data.playerRad;

        var collide = false;
        
        collide |= (posA - ppos).magnitude < (tumbaCollisionRad + prad);
        collide |= (posB - ppos).magnitude < (tumbaCollisionRad + prad);

        if (collide) // сразу проверим, что врезались в тумбы
            return true;
        
        if (mSinglePlace) // если тумбы в одном месте и в них не врезались, то проверять луч смысла нет.
            return false;

        ppos -= posA;

        float mag = ppos.magnitude; // длина вектора точки
        float num = mag * Vector3.Dot(mDir, ppos.normalized); // проекция вектора точки на линию лазера

        if (num > 0 && num < mLength) // проекция точки игрока лежит между тумбами
        {
            float a = Mathf.Abs(ppos.x * mDir.z - ppos.z * mDir.x); // расстояние до точки [ c * (1 - cosx^2) = c * sinx ]
            if (a < 0.25f + prad)
                return true;
        }
        return false;
    }

    public override void XUpdate(float dt, UpdateData data)
    {
    }

    
    public void Destroy()
    {
        Object.Destroy(sceneObject);
    }
    
}
