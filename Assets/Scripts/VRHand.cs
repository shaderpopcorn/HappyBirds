using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public enum Handness
{
    Right,
    Left
}


public class VRHand: MonoBehaviour
{
    [Header("Type")]
    // Enum to determine whether this is the left or right hand
    public Handness handness;

    [Header("Pickup Control")]

    [Space(50)]

    [Header("Scene Refs")]
    // Reference to the empty gameObject represting where held objects
    // will be anchored to
    public Transform holdPosition;
    private Remote remote = null;
    public Transform vrRig;

    [Header("Debug View")]
    // The object we are currently hovering over
    public Transform hoveredObject; // (only public so we can see it in play mode for debugging)

    // A boolean to keep track of if we are currently holding any object
    public bool isHolding = false; // (only public so we can see it in play mode for debugging)

    public Transform heldObject;

    [Header("Throw Control")]

    // Reference to animator component on the hand
    private Animator anim;

    // visual reference for the teleport position 
    public Transform teleportVisualRef;


    public float smoothnessValue = 0.2f;

    public LayerMask layerMask;

    private Vector3 lastPosition;
    public float throwForce = 20;

    public Collider collisionCollider;

    public RawImage fadeScreen;


    // Start is called before the first frame update
    void Start()
    {
        // Get the reference to the animator component of the hand
        anim = GetComponent<Animator>();

        lastPosition = transform.position;
        collisionCollider.enabled = false;
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
                //Ray ray = new Ray(transform.position, transform.forward);
                //RaycastHit hitInfo = new RaycastHit();
                //if (Physics.Raycast(ray, out hitInfo, 100, layerMask))
                //{
                //   // Start coroutine
                //   StartCoroutine(SmoothMoveToHand(hitInfo.collider.transform));
                //}

                collisionCollider.enabled = true;
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
            collisionCollider.enabled = false;

            XRNode nodeType;
            if (handness == Handness.Right)
            {
                nodeType = XRNode.RightHand;
            }
            else
            {
                nodeType = XRNode.LeftHand;
            }

            List<XRNodeState> nodeStates = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);

            for (int i = 0; i < nodeStates.Count; i++)
            {
                if(nodeStates[i].nodeType == nodeType)
                {
                    Vector3 velocity;
                    if (nodeStates[i].TryGetVelocity(out velocity))
                    {
                        heldObject.GetComponent<Rigidbody>().velocity = velocity * throwForce;
                    }
                }
            }

            isHolding = false;
            heldObject = null;
            remote = null;
        }

        if(Input.GetButtonDown("Fire") && remote != null)
        {
            remote.OnHitBigRedButton();
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
                    Vector3 newPos = new Vector3( hitInfo.point.x, vrRig.position.y, hitInfo.point.z );
                    StartCoroutine(MoveWithFade(newPos));
                }
            }
            else
            {
                teleportVisualRef.gameObject.SetActive( false );
            }
        }

        lastPosition = transform.position;
    }

    private void GrabObject(Transform objectToGrab)
    {
        remote = objectToGrab.GetComponent<Remote>();

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

    private IEnumerator MoveWithFade(Vector3 pos)
    {
        
        fadeScreen.color = Color.clear;
        float currentTime = 0;
        while (currentTime < 1)
        {
            fadeScreen.color = Color.Lerp(Color.clear, Color.black, currentTime);
            yield return null;
            currentTime += Time.deltaTime;
        }

        vrRig.position = pos;

        yield return new WaitForSeconds(0.4f);

        currentTime = 0;
        while (currentTime < 1)
        {
            fadeScreen.color = Color.Lerp(Color.black, Color.clear, currentTime);
            yield return null;
            currentTime += Time.deltaTime;
        }
        fadeScreen.color = Color.clear;
    }

    public Vector3 LerpV(Vector3 a, Vector3 b, float t)
    {
        return new Vector3( Lerp(a.x, b.x, t), Lerp(a.y, b.y, t), Lerp(a.z, b.z, t) );
    }

    
}
