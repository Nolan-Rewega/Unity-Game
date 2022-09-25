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

    private float currMovementModifier;
    private float prevMovementModifier;
    private float sensitivityMouse;
    private float sensitivityKey;
    private float phyObjDistance;
    private float currCameraY;
    private float prevCameraY;
    private float sprint;

    private bool interpolating;
    private float elapsedTime;

    private bool recharging;


    void Start(){
        interactionState = INTERACTSTATE.IDLE;
        movementState    = MOVEMENTSTATE.WALK;

        currMovementModifier = 1.0f;
        prevMovementModifier = 1.0f;
        sensitivityMouse =  3.0f;
        sensitivityKey   = 0.01f;
        phyObjDistance   =  1.0f;
        currCameraY   = 1.0f;
        prevCameraY   = 1.0f;
        sprint        = 1.0f;


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
                    prevMovementModifier = currMovementModifier;
                    currMovementModifier = 0.5f;
                    
                    // -- rotate door.
                    Vector3 mouseDir = new Vector3(-mouseDelta.x, 0.0f, -mouseDelta.y);
                    Vector3 cameraForward = new Vector3(cameraObj.transform.forward.x, 0.0f, cameraObj.transform.forward.z);
                    currDoor.GetComponent<Door>().rotateDoor(cameraForward, mouseDir);
                }
                else{
                    prevMovementModifier = currMovementModifier;
                    currMovementModifier = 1.0f;

                    interactionState = INTERACTSTATE.IDLE;
                    currDoor   = null;
                }
                break;

            case INTERACTSTATE.HOLDING:

                if (Input.GetKey(KeyCode.E)) {
                    // -- Move the Object in front of the player.
                    currPhyObj.transform.position = (cameraObj.transform.forward * phyObjDistance) + cameraObj.transform.position;

                    if (Input.GetKey(KeyCode.R)) {
                        currPhyObj.transform.Rotate(new Vector3(mouseDelta.y, mouseDelta.x,0.0f));
                    }
                    else{
                        currPhyObj.transform.eulerAngles += new Vector3(0.0f, -mouseDelta.x, 0.0f);
                    }
                }
                else{
                    // -- Apply a force to the object.
                    currPhyObj.GetComponent<Rigidbody>().useGravity = true;
                    currPhyObj.GetComponent<Rigidbody>().AddForce((Vector3.up + cameraObj.transform.forward.normalized) * 100.0f);

                    // -- Drop the object
                    interactionState = INTERACTSTATE.IDLE;
                    currPhyObj = null;
                }
                break;
        }
    }

    void playerMovement() {
        float Z = 0, X = 0;
        float angle = cameraObj.transform.eulerAngles.y * Mathf.Deg2Rad;
        
        float sprintModifier = 1.0f;
        float speedMod = currMovementModifier;
        float height   = currCameraY;

        // -- Interperalation. Player Speed and Player Camera Height
        if (interpolating){
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.2f;

            if (t > 1.0f){
                interpolating = false;
                elapsedTime = 0.0f;
            }
            else {
                speedMod = Mathf.Lerp(prevMovementModifier, currMovementModifier, t);
                height   = Mathf.Lerp(prevCameraY, currCameraY, t);
            }
        }

        // -- Sprint cooldown.
        if (recharging && playerStats.getPlayerStamina() > 25.0f) { 
            recharging = false; 
        }
        else if (Input.GetKey(KeyCode.LeftShift) && playerStats.getPlayerStamina() > 0.0f && !recharging){
            playerStats.setRunningState(true);
            sprintModifier *= 1.5f;
        }
        else if (playerStats.getPlayerStamina() < 0.1f) {
            recharging = true;
            playerStats.setRunningState(false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)){
            playerStats.setRunningState(false);
        }


        // -- Change movement state based on key input.

        if ( (Input.GetKeyDown(KeyCode.LeftControl) && movementState == MOVEMENTSTATE.CROUCH) ||
             (Input.GetKeyDown(KeyCode.C)           && movementState == MOVEMENTSTATE.PRONE ) ){
            setMovementState(MOVEMENTSTATE.WALK, 1.0f, 1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)) {
            setMovementState(MOVEMENTSTATE.CROUCH, 0.5f, 0.6f);
        }
        else if (Input.GetKeyDown(KeyCode.C)){
            setMovementState(MOVEMENTSTATE.PRONE, 0.2f, 0.3f);
        }

     
        // -- Apply movement modifiers
        Z += (Input.GetKey(KeyCode.W)) ? sensitivityKey * speedMod * sprintModifier : 0.0f;
        Z -= (Input.GetKey(KeyCode.S)) ? sensitivityKey * speedMod * sprintModifier : 0.0f;
        X += (Input.GetKey(KeyCode.D)) ? sensitivityKey * speedMod * sprintModifier : 0.0f;
        X -= (Input.GetKey(KeyCode.A)) ? sensitivityKey * speedMod * sprintModifier : 0.0f;

        gameObject.transform.position += new Vector3( Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z,
                                                      0.0f,
                                                      Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z );

        cameraObj.transform.position = new Vector3(cameraObj.transform.position.x, height, cameraObj.transform.position.z);
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


    private void setMovementState(MOVEMENTSTATE state, float height, float modifier) {
        if      (state == MOVEMENTSTATE.WALK)   { playerStats.setMovementState(0); }
        else if (state == MOVEMENTSTATE.CROUCH) { playerStats.setMovementState(1); }
        else if (state == MOVEMENTSTATE.PRONE)  { playerStats.setMovementState(2); }
        movementState = state;

        prevMovementModifier = currMovementModifier;
        currMovementModifier = modifier;

        prevCameraY = currCameraY;
        currCameraY = height;

        interpolating = true;
    }
}
