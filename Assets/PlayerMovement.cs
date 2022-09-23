using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // -- Private attributes.
    private enum INTERACTSTATE { IDLE, DOOR, HOLDING };
    private enum MOVEMENTSTATE { WALK, CROUCH, PRONE };

    private INTERACTSTATE interactionState;
    private MOVEMENTSTATE movementState;


    private GameObject cameraObj;
    private GameObject currPhyObj;
    private GameObject currDoor;

    private RayCastSelection raycaster;
    private PlayerStats playerStats;

    private Vector3 mouseDelta;

    private float sensitivityMouse;
    private float sensitivityKey;
    private float phyObjDistance;
    private float sprint;

    private float elapsedTime;
    private bool recharging;


    void Start(){
        interactionState = INTERACTSTATE.IDLE;
        movementState    = MOVEMENTSTATE.WALK;

        sensitivityMouse =  3.0f;
        sensitivityKey   = 0.01f;
        phyObjDistance   =  1.0f;
        sprint           =  1.0f;

        raycaster   = gameObject.GetComponent<RayCastSelection>();
        playerStats = gameObject.GetComponent<PlayerStats>();
        cameraObj   = gameObject.transform.GetChild(0).gameObject;
    }


    void Update(){
        mouseDelta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f) * sensitivityMouse;

        playerMovement();
        playerCamera();
        playerSelection();

    }


    void playerSelection(){

        switch (interactionState) {
            case INTERACTSTATE.IDLE:
                if (Input.GetKeyDown(KeyCode.E)) { raycaster.castRay(); }
                break;

            case INTERACTSTATE.DOOR:
                if (Input.GetKey(KeyCode.E)){
                    // -- rotate door.
                    Vector3 mouseDir = new Vector3(mouseDelta.x, 0.0f, mouseDelta.y) * Mathf.Deg2Rad;
                    currDoor.GetComponent<Door>().rotateDoor(gameObject.transform.forward, mouseDir);
                }
                else if (Input.GetKeyUp(KeyCode.E)) {
                    interactionState = INTERACTSTATE.IDLE;
                    currPhyObj   = null;
                }
                break;

            case INTERACTSTATE.HOLDING:
                if (Input.GetKeyUp(KeyCode.E)){
                    // -- Apply a force to the object.
                    currPhyObj.GetComponent<Rigidbody>().useGravity = true;
                    currPhyObj.GetComponent<Rigidbody>().AddForce((Vector3.up + cameraObj.transform.forward.normalized) * 100.0f);

                    // -- Drop the object
                    interactionState = INTERACTSTATE.IDLE;
                    currDoor     = null;
                }
                else {
                    // -- Move the Object in front of the player.
                    currPhyObj.transform.position = (cameraObj.transform.forward * phyObjDistance) + cameraObj.transform.position;

                    if (Input.GetKey(KeyCode.R)) {
                        currPhyObj.transform.Rotate(new Vector3(mouseDelta.y, mouseDelta.x,0.0f));
                    }
                    else{
                        currPhyObj.transform.eulerAngles += new Vector3(0.0f, -mouseDelta.x, 0.0f);
                    }
                    
                }
                break;
        }
    }

    void playerMovement() {
        float Z = 0, X = 0, Y = 0;
        float angle = cameraObj.transform.eulerAngles.y * Mathf.Deg2Rad;
        float movementPenalty = (interactionState == INTERACTSTATE.DOOR) ? 0.5f : 1.0f;


        // -- Sprint cooldown.
        if (recharging && playerStats.getPlayerStamina() > 25.0f) { 
            recharging = false; 
        }
        else if (Input.GetKey(KeyCode.LeftShift) && playerStats.getPlayerStamina() > 0.0f && !recharging){
            playerStats.setRunningState(true);
            movementPenalty *= 1.5f;
        }
        else if (playerStats.getPlayerStamina() < 0.1f) {
            recharging = true;
            playerStats.setRunningState(false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)){
            playerStats.setRunningState(false);
        }

        // -- YUCKY Changing movementState.
        switch (movementState) {
            case MOVEMENTSTATE.WALK:
                if (Input.GetKeyDown(KeyCode.LeftControl)) { 
                    movementState = MOVEMENTSTATE.CROUCH;
                    playerStats.setMovementState(1);
                    movementPenalty *= 0.6f;
                }
                else if (Input.GetKeyDown(KeyCode.C)) { 
                    movementState = MOVEMENTSTATE.PRONE;
                    playerStats.setMovementState(2);
                    movementPenalty *= 0.6f;
                }
                Y = 1.0f;
                movementPenalty *= 1.0f;
                break;

            case MOVEMENTSTATE.CROUCH:
                if (Input.GetKeyUp(KeyCode.LeftControl)) { 
                    movementState = MOVEMENTSTATE.WALK;
                    playerStats.setMovementState(0);
                }
                else if (Input.GetKeyDown(KeyCode.C)) { 
                    movementState = MOVEMENTSTATE.PRONE;
                    playerStats.setMovementState(2);
                }
                Y = 0.5f;
                movementPenalty *= 0.6f;
                break;

            case MOVEMENTSTATE.PRONE:
                if (Input.GetKeyUp(KeyCode.C)){
                    movementState = MOVEMENTSTATE.WALK;
                    playerStats.setMovementState(0);
                }
                else if (Input.GetKeyDown(KeyCode.LeftControl)) { 
                    movementState = MOVEMENTSTATE.CROUCH;
                    playerStats.setMovementState(1);
                }
                Y = 0.2f;
                movementPenalty *= 0.3f;
                break;
        }

        Debug.Log(movementPenalty);
        // -- Apply movement modifiers
        Z += (Input.GetKey(KeyCode.W)) ? sensitivityKey * movementPenalty : 0.0f;
        Z -= (Input.GetKey(KeyCode.S)) ? sensitivityKey * movementPenalty : 0.0f;
        X += (Input.GetKey(KeyCode.D)) ? sensitivityKey * movementPenalty : 0.0f;
        X -= (Input.GetKey(KeyCode.A)) ? sensitivityKey * movementPenalty : 0.0f;

        gameObject.transform.position += new Vector3( Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z,
                                                      0.0f,
                                                      Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z );

        cameraObj.transform.position = new Vector3(cameraObj.transform.position.x, Y, cameraObj.transform.position.z);
    }


    void playerCamera() {
        float x = mouseDelta.y;

        if (cameraObj.transform.eulerAngles.x + mouseDelta.y > 70.0f && cameraObj.transform.eulerAngles.x + mouseDelta.y < 290.0f) {
            x = 0.0f;
        }

        switch (interactionState) {

            case INTERACTSTATE.IDLE:
                cameraObj.transform.eulerAngles += new Vector3(x, -mouseDelta.x, 0.0f);

                break;

            case INTERACTSTATE.DOOR:
                break;

            case INTERACTSTATE.HOLDING:
                if (!Input.GetKey(KeyCode.R)){
                    cameraObj.transform.eulerAngles += new Vector3(x, -mouseDelta.x, 0.0f);
                }
                break;

        }
    }


    // -- Ugly Ugly spaghetti functions.
    public void updateDoorMode(GameObject door) {
        interactionState = INTERACTSTATE.DOOR;
        currDoor = door;
    }

    public void updateHeldObject(GameObject physicObject){
        interactionState = INTERACTSTATE.HOLDING;
        currPhyObj = physicObject;

        // -- Disable gravity
        currPhyObj.GetComponent<Rigidbody>().useGravity = false;
    }


}
