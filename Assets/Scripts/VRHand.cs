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
    public Handness handness;

    // Reference to the empty gameObject represting where held objects
    // will be anchored to
    public Transform holdPosition;

    // The object we are currently hovering over
    public Transform hoveredObject; // (only public so we can see it in play mode for debugging)

    // A boolean to keep track of if we are currently holding any object
    public bool isHolding = false; // (only public so we can see it in play mode for debugging)

    // visual reference for the teleport position 
    public Transform teleportVisualRef;

    public Transform vrRig;

    public float smoothnessValue = 0.2f;

    public LayerMask layerMask;

    public Transform heldObject;

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
        if (Input.GetButtonDown(handness + "Grip"))
        {
            anim.SetBool("GripPressed", true);

            if (hoveredObject != null) {
                // Pick up object
                // Parent to holdPosition transform ref, zero out its transform and turn of gravity
                GrabObject(hoveredObject);
            }
            else
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo, 100, layerMask))
                {
                   // Start coroutine
                   StartCoroutine(SmoothMoveToHand(hitInfo.collider.transform));
                }
            }
        }

        // IF the grip button of the proper hand is released run the open animation
        if( Input.GetButtonUp( handness + "Grip" ) )
        {
            anim.SetBool( "GripPressed", false );

            // Drop an object here (if holding one)
            // Set the parent of the held object to empty and turn gravity back on
            heldObject.SetParent( null );
            heldObject.GetComponent<Rigidbody>().useGravity = true;

            isHolding = false;
        }

        // Raycasting from the right hand
        if (handness == Handness.Right)
        {
            Ray ray = new Ray( transform.position, transform.forward );
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo)) // if youo use the 'out' keyword, yoou must change the variable
            { 
                teleportVisualRef.gameObject.SetActive( true );
                Vector3 desiredPosition = hitInfo.point;
                Vector3 vecToDesired = desiredPosition - teleportVisualRef.position;
                vecToDesired *= smoothnessValue;
                teleportVisualRef.position += vecToDesired;
                if (Input.GetButtonDown( handness + "Trigger" ))
                {
                    vrRig.position = new Vector3( hitInfo.point.x, vrRig.position.y, hitInfo.point.z );
                }
            }
            else
            {
                teleportVisualRef.gameObject.SetActive( false );
            }
        }
    }

    private void GrabObject(Transform objectToGrab)
    {
        heldObject = objectToGrab;
        heldObject.SetParent(holdPosition);
        heldObject.GetComponent<Rigidbody>().useGravity = false;
        heldObject.localPosition = Vector3.zero;
        heldObject.localRotation = Quaternion.identity;

        isHolding = true;
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

    private IEnumerator SmoothMoveToHand(Transform objectToMove)
    {
        float currentTime = 0;
        Vector3 startPos = objectToMove.position; // we have to define it outside, so the 'objectToMove' is not moving
        Vector3 endPos = holdPosition.position;
        while (currentTime < 1)
        {
            // your own update loop
            objectToMove.position = Vector3.Lerp(startPos, endPos, currentTime);

            yield return null; // leave and come back in the next frame
            
            currentTime += Time.deltaTime; // linear
            currentTime += Time.deltaTime + currentTime; // exponential
        }
        
        GrabObject(objectToMove);
    }

    // this is the way to bring in other functions
    public float Lerp(float a, float b, float t)
    {
        return ((b - a) * t) + a;
    }

    public Vector3 LerpV(Vector3 a, Vector3 b, float t)
    {
        return new Vector3( Lerp(a.x, b.x, t), Lerp(a.y, b.y, t), Lerp(a.z, b.z, t) );
    }
}
