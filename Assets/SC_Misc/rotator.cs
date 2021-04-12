using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour
{
    public float angular;
    float grad;

    private void Start()
    {
        grad = transform.position.z * 20;
    }

    void Update()
    {
        //  Здесь мы уже различаем поведение в редакторе и в рантайме
        if (!G.isRoundStarted)
            return;

        grad += angular * G.deltaTime;// Time.deltaTime;
        transform.localRotation = Quaternion.AngleAxis(grad, Vector3.up);
    }
}
