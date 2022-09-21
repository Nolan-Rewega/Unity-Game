using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float sprintModifier;

    private Vector3 previousPos;
    private Vector3 currentPos;

    private float sensitivityMouse;
    private float sensitivityKey;

    private bool doorMode;
    private GameObject currDoor;

    private RayCastSelection raycaster;

    private enum STATE { READY, DOOR };
    private STATE currentState;

    void Start(){
        currentPos  = Input.mousePosition;
        previousPos = Input.mousePosition;
        
        sensitivityMouse = 0.5f;
        sensitivityKey   = 0.01f;

        currentState = STATE.READY;

        raycaster = gameObject.GetComponent<RayCastSelection>();
    }


    void Update(){
        currentPos = Input.mousePosition;

        playerMovement();
        playerSelection();

        previousPos = currentPos;
    }

    void playerSelection(){
        if (Input.GetKeyDown(KeyCode.E) && currentState == STATE.READY)
        {
            raycaster.castRay();
        }
        else if (Input.GetKey(KeyCode.E) && currentState == STATE.DOOR)
        {
            // -- rotate door.
            Vector3 data = (previousPos - currentPos) * Mathf.Deg2Rad;
            Vector3 mouseDelta = new Vector3(-data.x, 0.0f, data.y); 

            currDoor.GetComponent<Door>().rotateDoor(gameObject.transform.forward, mouseDelta);
        }
        else if (Input.GetKeyUp(KeyCode.E) && currentState == STATE.DOOR)
        {
            currentState = STATE.READY;
            currDoor = null;
        }
    }

    void playerMovement(){
        // -- Cursor data.

        Vector3 mouseDelta = (previousPos - currentPos) * sensitivityMouse;

        float Z = 0, X = 0;
        if (currentState == STATE.READY){
 
            // -- Player movement.  W = +z, S = -Z, A = -x, D = +x;
            float sprint = (Input.GetKey(KeyCode.LeftShift)) ? 1.0f * sprintModifier : 1.0f;

            Z += (Input.GetKey(KeyCode.W)) ? 1.0f * sensitivityKey * sprint : 0.0f;
            Z -= (Input.GetKey(KeyCode.S)) ? 1.0f * sensitivityKey * sprint : 0.0f;
            X += (Input.GetKey(KeyCode.D)) ? 1.0f * sensitivityKey * sprint : 0.0f;
            X -= (Input.GetKey(KeyCode.A)) ? 1.0f * sensitivityKey * sprint : 0.0f;


            float angle = gameObject.transform.eulerAngles.y * Mathf.Deg2Rad;
            gameObject.transform.eulerAngles += new Vector3(mouseDelta.y, -mouseDelta.x, 0.0f);
            gameObject.transform.position += new Vector3(Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z, 0.0f, Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z);

        }
        else if (currentState == STATE.DOOR){

            Z += (Input.GetKey(KeyCode.W)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;
            Z -= (Input.GetKey(KeyCode.S)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;
            X += (Input.GetKey(KeyCode.D)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;
            X -= (Input.GetKey(KeyCode.A)) ? 1.0f * sensitivityKey / 2.0f : 0.0f;

            // -- Always look toward the door.
            gameObject.transform.position += new Vector3(X, 0, Z);
            //gameObject.transform.position += new Vector3(Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z, 0.0f, Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z);
        }
    }

    // -- Ugly Ugly spaghetti code.
    public void updateDoorMode(GameObject door) {
        currentState = STATE.DOOR;
        currDoor = door;
    }

}
