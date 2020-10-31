using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    // Run the collected animation when someone enters the coin
    private void OnTriggerEnter( Collider other )
    {
        GetComponent<Animator>().SetTrigger( "Collected" );
    }
}
