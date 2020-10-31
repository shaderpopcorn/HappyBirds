using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Handness
{
    Right,
    Left
}


public class VRHand: MonoBehaviour
{
    // Reference to animator component on the hand
    private Animator anim;

    // Enum to determine whether this is the left or right hand
    public Handness handess;

    // Reference to the empty gameObject represting where held objects
    // will be anchored to
    public Transform holdPosition;

    // The object we are currently hovering over
    public Transform hoveredObject; // (only public so we can see it in play mode for debugging)

    // A boolean to keep track of if we are currently holding any object
    public bool isHolding = false; // (only public so we can see it in play mode for debugging)

    // Start is called before the first frame update
    void Start()
    {
        // Get the reference to the animator component of the hand
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // IF the grip button of the proper hand is pressed run the close animation
        if( Input.GetButtonDown( handess + "Grip" ) )
        {
            anim.SetBool( "GripPressed", true );
        }

        // IF the grip button of the proper hand is released run the open animation
        if( Input.GetButtonUp( handess + "Grip" ) )
        {
            anim.SetBool( "GripPressed", false );
        }

        // If the trigger of the correct hand is pressed
        if( Input.GetButtonDown( handess + "Trigger" ) )
        {
            // Pick up object
            if( hoveredObject != null )
            {
                // Parent to holdPosition transform ref, zero out its transform and turn of gravity
                hoveredObject.SetParent( holdPosition );
                hoveredObject.GetComponent<Rigidbody>().useGravity = false;
                hoveredObject.localPosition = Vector3.zero;
                hoveredObject.localRotation = Quaternion.identity;

                isHolding = true;
            }
        }

        // If the trigger is released
        if( Input.GetButtonUp( handess + "Trigger" ) )
        {
            // Drop an object here (if holding one)
            if( isHolding == true )
            {
                // Set the parent of the held object to empty and turn gravity back on
                hoveredObject.SetParent( null );
                hoveredObject.GetComponent<Rigidbody>().useGravity = true;

                isHolding = false;
            }
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        // Set the currently hovered object if it is something we can pick up
        if( other.tag == "Interactable" )
        {
            hoveredObject = other.transform;
        }
    }

    private void OnTriggerExit( Collider other )
    {
        // Un set the currently hovered object if it is the one we just exitededededed....ed
        if( other.transform == hoveredObject )
        {
            hoveredObject = null;
        }
    }
}
