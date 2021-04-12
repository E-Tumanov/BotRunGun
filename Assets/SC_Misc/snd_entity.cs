using UnityEngine;
using System.Collections;

public class snd_entity : MonoBehaviour
{

    public AudioSource a;
    public float ttl = 0;
    public float delay = 0;

    void Update()
    {
        if (a == null)
        {
            Destroy(transform.gameObject);
            return;
        }

        delay -= Time.deltaTime;
        if (delay < 0 && !a.isPlaying)
            a.Play();

        ttl -= Time.deltaTime;
        if (ttl < 0)
        {
            //Destroy (transform.gameObject);
            a.Stop();
        }
    }
}
