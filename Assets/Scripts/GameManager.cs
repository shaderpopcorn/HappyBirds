using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Assest reference from the project window for the structure to spawn
    public GameObject structurePrefab;

    // Transform reference, to know where to spawn the structure
    public Transform structurePositionRef;

    private void Start()
    {
        // Reset the game on start
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset the game is the R key is pressed
        if (Input.GetKeyDown("r"))
        {
            ResetGame();
        }
    }

    private void ResetGame()
    {
        // Destroy and previous structure that is still around
        if( structurePositionRef.childCount > 0 )
        {
            Destroy( structurePositionRef.GetChild( 0 ).gameObject );
        }

        // Create a new structure at the structurePostionRef transform
        Instantiate( structurePrefab, structurePositionRef.position, Quaternion.identity, structurePositionRef );
    }
}
