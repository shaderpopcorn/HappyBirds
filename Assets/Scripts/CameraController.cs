using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 3f;

    // Update is called once per frame
    void Update()
    {
        // Move the camera forward back left and right relative to where it is facing
        if( Input.GetKeyDown( "w" ) )
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if( Input.GetKey( "s" ) )
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
        else if( Input.GetKey( "d" ) )
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }
        else if( Input.GetKey( "a" ) )
        {
            transform.position -= transform.right * moveSpeed * Time.deltaTime;
        }

        // Rotate camera based off of mouse movement
        transform.Rotate( 0, Input.GetAxis( "Mouse X" ) * mouseSensitivity, 0, Space.World );
        transform.Rotate( -Input.GetAxis( "Mouse Y" ) * mouseSensitivity, 0, 0, Space.Self );
        
    }


}
