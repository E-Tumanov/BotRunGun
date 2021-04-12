using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

/// <summary>
/// 2 тумбы и энергетический шар между ними
/// </summary>
public interface IWalkBall
{
    void SetAPos(Vector3 pos);
    void SetBPos(Vector3 pos);
    void DestroyIt();
}

public class ItemWalkBall : Item, IWalkBall
{
    [SerializeField] Transform p1;
    [SerializeField] Transform p2;
    [SerializeField] Transform ball;
    [SerializeField] float tumbaCollisionRad = 0.4f;
    [SerializeField] float ballCollisionRad = 0.5f;

    float ballSpeed;
    Vector3 mStartPivotPos;

    Vector3 dir;
    float length;
    bool singlePlace = false;

    public override Vector3 pos => mStartPivotPos;
    public override bool IsUpdated() => true;

    public override void Init(JSONNode param)
    {
        float ypos1 = 0;
        float ypos2 = 0;
        if (G.isRoundStarted)
        {
            ypos1 = di.GetUpOffset (param[1].AsFloat);
            ypos2 = di.GetUpOffset (param[1].AsFloat + param[3].AsFloat);
        }

        mStartPivotPos = new Vector3(param[0].AsFloat, ypos1, param[1].AsFloat);
        p1.position = mStartPivotPos;
        p2.position = new Vector3(mStartPivotPos.x + param[2].AsFloat, ypos2, mStartPivotPos.z + param[3].AsFloat);
        ballSpeed = param[4].AsFloat;

        SolveData();
        SolveBallPos();
    }

    public void SetAPos(Vector3 pos) 
    {
        p1.position = pos;
        SolveData();
        SolveBallPos();
    }

    public void SetBPos(Vector3 pos) 
    {
        p2.position = pos;
        SolveData();
        SolveBallPos();
    }
    

    void SolveData()
    {
        if ((p1.transform.position - p2.transform.position).magnitude > 0.25f)
        {
            dir = p2.position - p1.position;
            length = dir.magnitude;
            dir /= length;
        }
        else
        {
            singlePlace = true;
            length = 0.001f;
            dir = Vector3.one;
        }
    }

    public void DestroyIt()
    {
        Object.Destroy(sceneObject);
    }

    void SolveBallPos()
    {
        if (singlePlace)
            ball.position = p1.position;
        else
        {
            ball.position = p1.position + length * dir * (0.5f + 0.5f * Mathf.Sin(3 * ballSpeed * 3.1415f * G.time / length));
        }
    }

    public override bool XCollision(UpdateData data)
    {
        //  не доехали даже до минимальной тумбы
        if (transform.position.z - data.playerPos.y > 5)
            return false;

        Vector3 ppos = new Vector3(data.playerPos.x, 0, data.playerPos.y);
        float prad = data.playerRad;

        var collide = false;
        collide |= (p1.position - ppos).magnitude < (tumbaCollisionRad + prad);
        collide |= (p2.position - ppos).magnitude < (tumbaCollisionRad + prad);

        if (collide) // сразу проверим, что врезались в тумбы
            return true;

        Vector2 ppos2 = data.playerPos;
        return (new Vector2(ball.position.x, ball.position.z) - ppos2).magnitude < (ballCollisionRad + prad);
    }

    public override void XUpdate(float dt, UpdateData data)
    {
        //SolveData();
        SolveBallPos();
    }
}
