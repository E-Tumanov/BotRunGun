using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoadData", menuName = "ScriptableObjects/CfgRoad", order = 1)]
public class CfgRoad : ScriptableObject
{
    public AnimationCurve roadY;
    public AnimationCurve roadX;

    public float UCoordScale;
    
    public float AmpScaleX;
    public float AmpScaleY;

    public float NormalDistance = 500;

    public float GetUpOffset(float currZ)
    {
        float u = UCoordScale * (currZ) / (0.001f + NormalDistance);
        return 0;// roadY.Evaluate (u) * AmpScaleY - 0.10f * currZ; // чтоб всё время под уклон
    }

    public List<Color32> ColorStack;

    public List<Texture> ColorWallList;
}
