using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
========================================
Слой анимации(тряска) для камеры
========================================
*/
class JitLayer
{
    float amp = 0.1f;
    float dtime = 0.5f;
    float hardMove1000 = 1;

    Vector3 tPos;
    Vector3 vPos;
    float tmrNewPos;
    float tmrDelta;

    Vector3 targetPos;
    public JitLayer(float amplitude, float durTime, float hmove)
    {
        amp = amplitude;
        dtime = durTime;
        hardMove1000 = hmove;
    }

    public Vector3 solve()
    {
        if (Time.time >= tmrNewPos-1)
        {
            tmrDelta = 0.01f + dtime / 1000.0f;
            tmrNewPos = Time.time + tmrDelta;
            vPos = tPos;
            tPos = Random.onUnitSphere * amp;
        }

        float t = 1 - (tmrNewPos - Time.time) / tmrDelta;

        Vector3 pp = Vector3.Lerp(vPos, tPos, t);

        float hh = hardMove1000 / 1000.0f;
        hh *= hh;

        targetPos = (1 - hh) * targetPos + hh * pp;
        return targetPos;
    }
}


/*
========================================
Система тряски камеры
========================================
*/
public class CameraJitter : MonoBehaviour
{
    public float amp = 0.1f;
    public float dtime = 0.5f;
    public float hardMove1000 = 1;

    //  интерполятор джита. от малого к мощному
    public float jitLerp;

    float jitTime;
    
    static CameraJitter jitCam;

    static JitLayer lowJit;
    static JitLayer hiJit;

    Vector3 startLocalPos;

    void Start()
    {
        jitCam = this;
        startLocalPos = transform.localPosition;

        lowJit = new JitLayer(amp, dtime, hardMove1000);
        hiJit = new JitLayer(0.5f, 0.01f, 400);
        jitLerp = 0;
    }


    void Update()
    {
        jitTime -= Time.deltaTime;
        jitLerp = Mathf.Clamp01(jitTime);

        transform.localPosition = startLocalPos + Vector3.Lerp(lowJit.solve(), hiJit.solve(), jitLerp);
    }

    void jit_it(float sec)
    {
        if (jitTime > 0)
            jitTime += sec;
        else
            jitTime = sec;
    }

    //  Трясти камеру
    public static void JitIt(float sec)
    {
        jitCam.jit_it(sec);
    }

    //  jitter TIME. Сам тайм всегда уменьшается.
    public static float JitPower
    {
        get 
        { 
            if (jitCam.jitTime<0)
                return 0;
            return jitCam.jitTime;
        }
        
        set 
        { 
            jitCam.jitTime = value; 
        }
    }
}