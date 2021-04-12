using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ААА, это шарики в стакане. 
/// </summary>
class TT
{
    public static int UID;

    public Vector3 pos;
    public Vector3 dir; // speed
    public float rad;
    public int id;
    public float maxSpeed;
    public float addDump;
    public TT() 
    {
        id = ++UID;

        pos = Random.onUnitSphere * 0.5f + new Vector3(0.5f, 0, 0.5f);
        pos.x *= 20;
        pos.z *= 20;
        pos.y = 0;

        dir = Random.onUnitSphere;
        dir.y = 0;

        rad = 0.2f + Random.value * 0.5f;
        maxSpeed = 4;
    }
}

public class balls : MonoBehaviour
{
    int count = 25;
    public float dump = 1;
    public float power = 1;

    List<TT> list = new List<TT>();

    void Start()
    {
        
        
        
        var a = new TT();
        var b = new TT();

        a.pos.x = 5;
        a.pos.z = 10;
        a.dir = new Vector3(1, 0, 0);
        a.rad = 2;
        a.maxSpeed = 0;

        b.pos.x = 10;
        b.pos.z = 10;
        b.dir = new Vector3(-1, 0, 0);
        b.rad = 0.5f;
        b.maxSpeed = 0;

        list.Add(a);
        list.Add(b);

        for (int i = 0; i < count; i++)
            list.Add(new TT());
    }

    private void OnDrawGizmos()
    {
        foreach (var p in list)
        {
            Gizmos.DrawWireSphere(p.pos, p.rad);
        }            
    }

    void SolveBounce(TT a, TT b)
    {
        if (a.id == b.id)
            return;

        Vector3 dir = (a.pos - b.pos);
        float mag = dir.magnitude;
        if (mag > a.rad + b.rad) // проверка соударения
            return;

        Vector3 force = dir.normalized * power * (a.rad + b.rad);
        a.dir += force;
        b.dir -= force;
    }

    void Solve(TT p, float dt)
    {
        p.pos += p.dir * dt;

        float speed = p.dir.magnitude;
        
        p.dir -= Mathf.Pow (1 + p.rad, dump) * p.dir * ((speed - p.maxSpeed) / 2.0f) * dt;

        if (p.pos.x < 0)
        {
            p.pos.x = 0;
            p.dir.x *= -1;
        }
        
        if (p.pos.x > 20)
        {
            p.pos.x = 20;
            p.dir.x *= -1;
        }

        if (p.pos.z < 0)
        {
            p.pos.z = 0;
            p.dir.z *= -1;
        }

        if (p.pos.z > 20)
        {
            p.pos.z = 20;
            p.dir.z *= -1;
        }
    }

    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                var p = list[j];
                
                Solve(p, Time.deltaTime / 10.0f);

                for (int k = j + 1; k < list.Count; k++)
                {
                    SolveBounce(p, list[k]);
                }
            }
        }
    }
}
