using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    // -- Private attributes.
    private enum INTERACTSTATE { IDLE, DOOR, HOLDING };
    private INTERACTSTATE interactionState;

    private GameObject cameraObj;
    private GameObject currPhyObj;
    private GameObject currDoor;

    private PlayerMovement player;
    private RayCastSelection raycaster;
    private Vector3 mouseDelta;

    private float sensitivityMouse;


    void Start(){
        interactionState = INTERACTSTATE.IDLE;

        sensitivityMouse = 3.0f;
        
        currDoor   = null;
        currPhyObj = null;

        raycaster = gameObject.GetComponent<RayCastSelection>();
        cameraObj = gameObject.transform.GetChild(0).gameObject;
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }


    void Update(){
        mouseDelta = sensitivityMouse * new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);

        playerSelection();
    }


    // -- Ugly Ugly spaghetti functions.
    public void updateDoorMode(GameObject door){
        interactionState = INTERACTSTATE.DOOR;
        currDoor = door;
        player.haltPlayerCamera(true);
    }

    public void updateHeldObject(GameObject physicObject){
        interactionState = INTERACTSTATE.HOLDING;
        currPhyObj = physicObject;

        // -- Disable gravity
        currPhyObj.GetComponent<Rigidbody>().useGravity = false;
    }




    private void playerSelection(){

        switch (interactionState){
            case INTERACTSTATE.IDLE:
                if (Input.GetKeyDown(KeyCode.E)) { raycaster.castRay(); }
                break;

            case INTERACTSTATE.DOOR:
                if (Input.GetKey(KeyCode.E)){
                    Debug.Log("test");
                    // -- rotate door.
                    Vector3 mouseDir = new Vector3(-mouseDelta.x, 0.0f, -mouseDelta.y);
                    Vector3 cameraForward = new Vector3(cameraObj.transform.forward.x, 0.0f, cameraObj.transform.forward.z);
                    currDoor.GetComponent<Door>().doorAction(cameraForward, mouseDir);
                }
                else{
                    interactionState = INTERACTSTATE.IDLE;
                    player.haltPlayerCamera(false);
                    currDoor = null;
                }
                break;

            case INTERACTSTATE.HOLDING:

                if (Input.GetKey(KeyCode.E)){
                    // -- Move the Object in front of the player.
                    currPhyObj.transform.position = (cameraObj.transform.forward * 1.0f) + cameraObj.transform.position;

                    if (Input.GetKey(KeyCode.R)){
                        currPhyObj.transform.Rotate(new Vector3(mouseDelta.y, mouseDelta.x, 0.0f));
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

}
