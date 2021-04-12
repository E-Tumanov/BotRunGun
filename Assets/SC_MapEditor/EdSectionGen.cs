using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


/// <summary>
/// Генератор секций в окрестностях курсора
/// </summary>
public interface IEdSectionGen
{
    void SetTargetPos(Vector3 pos);
}


public class EdSectionGen : MonoBehaviour, IEdSectionGen
{
    [SerializeField] Transform wallPrefab;

    float targetPos = -100;
    List<Transform> wallList = new List<Transform>();

    public void SetTargetPos(Vector3 pos)
    {
        if (Mathf.Abs(targetPos - pos.z) > 5)
        {
            targetPos = pos.z;
            GenWall();
        }
    }

    private void GenWall()
    {
        foreach (var e in wallList)
        {
            DestroyImmediate(e.gameObject);
        }

        wallList.Clear();
      
        for (int i = -1; i < 30; i++)
        {
            float d = i * 100;
            wallList.Add(Instantiate<Transform>(wallPrefab, Vector3.forward * d, Quaternion.identity));
        }
    }
}
