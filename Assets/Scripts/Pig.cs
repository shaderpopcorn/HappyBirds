using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    // Keep track of the time the pig came to life
    public float timeAtStart;

    public void Awake()
    {
        // Record the current time
        timeAtStart = Time.timeSinceLevelLoad;
    }


    private void OnCollisionEnter( Collision collision )
    {
        // Ignore any collision that happens within one second of the pig spawning
        // otherwise destroy the pig
        if (Time.timeSinceLevelLoad - timeAtStart > 1)
        {
            Destroy( gameObject );
        }
    }
}
