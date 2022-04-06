using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator_axes : MonoBehaviour
{
    public Vector3 angular;
    public bool ignorePauseGame = false;

    void Update()
    {
        if (!G.isRoundStarted && ignorePauseGame)
            return;
        transform.Rotate(angular * Time.deltaTime, Space.Self);
    }
}
