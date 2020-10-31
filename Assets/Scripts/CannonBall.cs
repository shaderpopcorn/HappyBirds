using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    // Counter to keep track of how many objects the cannon ball hit
    private int hitCount = 0;

    private void OnCollisionEnter( Collision collision )
    {
        // Every time a collision is registered add one to the hit count
        hitCount = hitCount + 1;

        // If we have hit something more than three times, destroy the cannonball
        if (hitCount >= 3)
        {
            Destroy( gameObject );
        }
    }
}
