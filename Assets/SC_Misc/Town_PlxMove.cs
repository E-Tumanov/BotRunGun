using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town_PlxMove : MonoBehaviour
{
    [SerializeField] Transform mainCamera;
    [SerializeField] Transform backCamera;

    Vector3 camStartPos;
    private void Start()
    {
        camStartPos = backCamera.position = mainCamera.position;
    }

    void Update()
    {
        var dir = mainCamera.position - camStartPos;
        dir.x *= 0.01f;
        dir.y *= 0.01f;
        dir.z *= 0.003f;
        backCamera.transform.position = camStartPos + dir;
    }
}
