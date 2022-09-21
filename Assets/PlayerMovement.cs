using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // -- Private attributes.
    private enum STATE { READY, DOOR, HOLDING };
    private STATE currentState;

    private GameObject playerCamera;
    private GameObject currPhyObj;
    private GameObject currDoor;
    private RayCastSelection raycaster;

    private Vector3 mouseDelta;

    private float sensitivityMouse;
    private float sensitivityKey;
    private float phyObjDistance;
    private float sprint;


    void Start(){
        currentState = STATE.READY;

        sensitivityMouse =  3.0f;
        sensitivityKey   = 0.01f;
        phyObjDistance   =  1.0f;
        sprint           =  1.0f;

        raycaster    = gameObject.GetComponent<RayCastSelection>();
        playerCamera = gameObject.transform.GetChild(0).gameObject;
    }


    void Update(){
        mouseDelta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f) * sensitivityMouse;
        sprint = (Input.GetKey(KeyCode.LeftShift)) ? 2.0f : 1.0f;
        
        playerMovement();
        playerSelection();

    }


    void playerSelection(){

        switch (currentState) {
            case STATE.READY:
                if (Input.GetKeyDown(KeyCode.E)) { raycaster.castRay(); }
                break;

            case STATE.DOOR:
                if (Input.GetKey(KeyCode.E)){
                    // -- rotate door.
                    Vector3 mouseDir = new Vector3(mouseDelta.x, 0.0f, mouseDelta.y) * Mathf.Deg2Rad;
                    currDoor.GetComponent<Door>().rotateDoor(gameObject.transform.forward, mouseDir);
                }
                else if (Input.GetKeyUp(KeyCode.E)) {
                    currentState = STATE.READY;
                    currPhyObj   = null;
                }
                break;

            case STATE.HOLDING:
                if (Input.GetKeyUp(KeyCode.E)){
                    // -- Apply a force to the object.
                    currPhyObj.GetComponent<Rigidbody>().useGravity = true;
                    currPhyObj.GetComponent<Rigidbody>().AddForce((Vector3.up + playerCamera.transform.forward.normalized) * 100.0f * sprint);

                    // -- Drop the object
                    currentState = STATE.READY;
                    currDoor     = null;
                }
                else {
                    // -- Move the Object in front of the player.
                    currPhyObj.transform.position = (playerCamera.transform.forward * phyObjDistance) + playerCamera.transform.position;
                    currPhyObj.transform.rotation = playerCamera.transform.rotation ;
                }
                break;

            default:
                break;
        }
    }

    void playerMovement(){
        float Z = 0, X = 0;
        if (currentState == STATE.READY || currentState == STATE.HOLDING){
            // -- Player movement.  W = +z, S = -Z, A = -x, D = +x;
            Z += (Input.GetKey(KeyCode.W)) ? 1.0f * sensitivityKey * sprint : 0.0f;
            Z -= (Input.GetKey(KeyCode.S)) ? 1.0f * sensitivityKey * sprint : 0.0f;
            X += (Input.GetKey(KeyCode.D)) ? 1.0f * sensitivityKey * sprint : 0.0f;
            X -= (Input.GetKey(KeyCode.A)) ? 1.0f * sensitivityKey * sprint : 0.0f;


            float angle = playerCamera.transform.eulerAngles.y * Mathf.Deg2Rad;

            playerCamera.transform.eulerAngles += new Vector3(mouseDelta.y, -mouseDelta.x, 0.0f);
            gameObject.transform.position += new Vector3( Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z,
                                                          0.0f,
                                                          Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z );

        }
        else if (currentState == STATE.DOOR){
            Z += (Input.GetKey(KeyCode.W)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;
            Z -= (Input.GetKey(KeyCode.S)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;
            X += (Input.GetKey(KeyCode.D)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;
            X -= (Input.GetKey(KeyCode.A)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;

            // -- Always look toward the door.
            float angle = playerCamera.transform.eulerAngles.y * Mathf.Deg2Rad;
            gameObject.transform.position += new Vector3( Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z,
                                                          0.0f,
                                                          Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z );
        }
    }

    // -- Ugly Ugly spaghetti functions.
    public void updateDoorMode(GameObject door) {
        currentState = STATE.DOOR;
        currDoor = door;
    }

    public void updateHeldObject(GameObject physicObject){
        currentState = STATE.HOLDING;
        currPhyObj = physicObject;

        // -- Disable gravity
        currPhyObj.GetComponent<Rigidbody>().useGravity = false;
    }

}
