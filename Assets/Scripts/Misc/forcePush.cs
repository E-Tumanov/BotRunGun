using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forcePush : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        Vector3 dir = 1f * Vector3.forward;
        dir += 0.3f * Random.value * Vector3.up;
        dir += 0.5f * Vector3.right * (2 * Random.value - 1);
        dir *= (1 + Random.value) * 10;
        rb.AddForce(dir, ForceMode.Impulse);
    }
}
