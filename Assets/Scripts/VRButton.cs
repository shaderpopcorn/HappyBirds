using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRButton : MonoBehaviour
{
    // Keep track of if the button is currently pressed
    private bool isPressed = false;

    // Get a reference to the animator component on the button
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Get the animator component and save it on start
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter( Collider other )
    {
        // If a VR controller enters the button and it is not currently pressed then run the pressed animation
        if (other.tag == "VRHand" && !isPressed )
        {
            anim.SetTrigger( "ButtonPressed" );
            isPressed = true;
        }
    }



    private void OnTriggerExit( Collider other )
    {
        // Once the hand leaves reset the pressed bool so that we can use the button again
        if( other.tag == "VRHand"  )
        {
            isPressed = false;
        }
    }
}
