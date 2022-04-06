using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
==========================================
Крутит объект по Y
==========================================
*/
public class AnmTorsoRotate : MonoBehaviour
{
    public float rotateSpeed;
    public float gradAmp;

    Quaternion angStart;
    Quaternion angEnd;
    
    float currTime;
    float intervalTime;
    float yy;

    float pauseTimer;

    void Start()
    {
        angStart = angEnd = transform.localRotation;
        yy = 0;
        pauseTimer = 0;
    }

    
    void Update()
    {
        if (pauseTimer > Time.time)
            return;
        
        currTime += Time.deltaTime;

        if (currTime > intervalTime)
        {
            
            angStart = transform.localRotation;
            float prev = yy;
            yy = gradAmp * (Random.value * 2 - 1);//  timeIntervalMin + (timeIntervalMax - timeIntervalMin) * Random.value;
            angEnd = Quaternion.Euler(Vector3.up * yy);
            
            currTime = 0;
            intervalTime = Mathf.Abs(prev - yy) / rotateSpeed;
            
            pauseTimer = Time.time + 1 + Random.value * 2;
        }
        else if (angStart != angEnd)
            transform.localRotation = Quaternion.Slerp(angStart, angEnd, currTime / intervalTime);
    }
}
