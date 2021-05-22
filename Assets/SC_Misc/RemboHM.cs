using RBGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://www.raywenderlich.com/3169311-runtime-mesh-manipulation-with-unity

public class RemboHM : GModel
{
    [SerializeField] Mesh meshOriginal;
    
    public CfgRoad cfg;

    public int colornum;

    private void OnValidate ()
    {
        
    }



    private void Start()
    {
        if (cfg == null)
            cfg = roadConfig;

        if (!cfg || !meshOriginal)
            return;

        var nmesh = Instantiate<Mesh>(meshOriginal);

        var v = nmesh.vertices;

        for (int i = 0; i < v.Length; i++)
        {
            //v[i].x += cvX.Evaluate(u) * AmpScaleX;
            //float u = UCoordScale * (transform.position.z + v[i].z) / (0.001f + cfg.NormalDistance);
            v[i].y += di.GetUpOffset (transform.position.z + v[i].z);//  cvY.Evaluate(u) * AmpScaleY;
        }

        nmesh.vertices = v;

        GetComponent<MeshFilter>().sharedMesh = nmesh;
        nmesh.RecalculateBounds();

        var num = Mathf.Clamp (colornum, 0, cfg.ColorWallList.Count - 1);
       //   Работало. просто пока убрал
        // GetComponent<MeshRenderer> ().material.mainTexture = cfg.ColorWallList[num];
    }
}
