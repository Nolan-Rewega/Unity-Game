using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MOVEMENTSTATE { WALK, CROUCH, PRONE, RUN, SLOWED };

    // -- Private attributes.
    private MOVEMENTSTATE movementState;
    private Dictionary<MOVEMENTSTATE, float[]> stateAttributes;

    private GameObject cameraObj;
    private PlayerStats playerStats;

    private Vector3 mouseDelta;

    private float currMovementModifier;
    private float prevMovementModifier;

    private float interpolatedHeight;
    private float interpolatedSpeed;

    // -- Player Movement flags.
    private bool haltedPlayerMovement;
    private bool haltedPlayerCamera;
    private bool haltedStateChange;
    private bool haltedSpeedChange;

    private float sensitivityMouse;
    private float sensitivityKey;
    private float currCameraY;
    private float prevCameraY;

    private bool interpolating;
    private float currDuration;
    private float elapsedTime;



    void Start(){
        movementState = MOVEMENTSTATE.WALK;

        haltedPlayerMovement = false;
        haltedPlayerCamera = false;
        haltedStateChange = false;
        haltedSpeedChange = false;

        stateAttributes = new Dictionary<MOVEMENTSTATE, float[]>()
        {   
            // -- states         ->  height, speed mod, lerp duration, stealth level.
            {MOVEMENTSTATE.WALK  , new float[] { 1.0f, 1.5f, 0.2f, 1.0f } },
            {MOVEMENTSTATE.CROUCH, new float[] { 0.5f, 0.7f, 0.2f, 0.5f } },
            {MOVEMENTSTATE.PRONE , new float[] { 0.2f, 0.3f, 0.3f, 0.2f } },
            {MOVEMENTSTATE.RUN   , new float[] { 1.0f, 3.0f, 0.2f, 4.0f } },
            {MOVEMENTSTATE.SLOWED, new float[] { 1.0f, 0.4f, 0.3f, 1.0f } }
        };

        currMovementModifier = 1.0f;
        prevMovementModifier = 1.0f;
        sensitivityMouse =  3.0f;
        sensitivityKey   =  0.01f;

        currCameraY   = 1.0f;
        prevCameraY   = 1.0f;

        elapsedTime = 0.0f;

        playerStats = gameObject.GetComponent<PlayerStats>();
        cameraObj   = gameObject.transform.GetChild(0).gameObject;
    }


    void Update(){
        mouseDelta = sensitivityMouse * new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);

        // -- Removed checkStealthMode();
        checkPlayerStamina();
        
        if (!haltedStateChange) { modifyMovementState(); }
        if (!haltedPlayerMovement) { movePlayer(); }
        if (!haltedPlayerCamera) { moveCamera(); }
    }


    public void setMovementState(MOVEMENTSTATE state) {
        float[] attributes = stateAttributes[state];

        // -- Update playerStats.
        playerStats.setStealthLevel(attributes[3]);
        playerStats.setRunningState((state == MOVEMENTSTATE.RUN));

        movementState = state;

        // -- Already interpolating so switch mid interpolation.
        if (interpolating){
            elapsedTime = 0.0f;
            prevMovementModifier = interpolatedSpeed;
            prevCameraY = interpolatedHeight;
        }
        else {
            prevMovementModifier = currMovementModifier;
            prevCameraY = currCameraY;
        }

        // -- Update player modifiers.
        currMovementModifier = attributes[1];
        currCameraY = attributes[0];

        interpolating = true;
        currDuration = attributes[2];
    }


    public void haltPlayerMovement(bool value) {
        haltedPlayerMovement = value;
    }
    public void haltPlayerCamera(bool value) {
        haltedPlayerCamera = value;
    }
    public void haltMovementStateChange(bool value) {
        haltedStateChange = value;
    }
    public void haltMovementSpeedChange(bool value){
        haltedSpeedChange = value;
    }




    private void modifyMovementState() {
        // -- If currently slower dont change states.
        if (movementState == MOVEMENTSTATE.SLOWED) { return; }

        // -- Change movement state based on key input.
        //    TODO: Expand to full state machine.
        if (( Input.GetKeyDown(KeyCode.LeftControl) && movementState == MOVEMENTSTATE.CROUCH) ||
            ( Input.GetKeyDown(KeyCode.C)           && movementState == MOVEMENTSTATE.PRONE )    ){
            setMovementState(MOVEMENTSTATE.WALK);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) {
            setMovementState(MOVEMENTSTATE.RUN);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && movementState == MOVEMENTSTATE.RUN || 
                 Input.GetKeyUp(KeyCode.W)         && movementState == MOVEMENTSTATE.RUN    ) {
            setMovementState(MOVEMENTSTATE.WALK);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)){
            setMovementState(MOVEMENTSTATE.CROUCH);
        }
        else if (Input.GetKeyDown(KeyCode.C)){
            setMovementState(MOVEMENTSTATE.PRONE);
        }

    }


    private void movePlayer() {
        float Z = 0.0f, X = 0.0f;
        float angle = cameraObj.transform.eulerAngles.y * Mathf.Deg2Rad;
        
        float speedMod = currMovementModifier;
        float height   = currCameraY;

        // -- Interperalate Player Speed and Player Camera Height
        if (interpolating){
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / currDuration;

            if (t > 1.0f){
                interpolating = false;
                elapsedTime = 0.0f;
            }
            else{
                interpolatedSpeed  = Mathf.Lerp(prevMovementModifier, currMovementModifier, t);
                interpolatedHeight = Mathf.Lerp(prevCameraY, currCameraY, t);
                height = interpolatedHeight;
                speedMod = interpolatedSpeed;
            }
        }

        // -- Get the direction
        Z += (Input.GetKey(KeyCode.W)) ? 1.0f : 0.0f;
        Z -= (Input.GetKey(KeyCode.S)) ? 1.0f : 0.0f;
        X += (Input.GetKey(KeyCode.D)) ? 1.0f : 0.0f;
        X -= (Input.GetKey(KeyCode.A)) ? 1.0f : 0.0f;

        Vector3 direction = sensitivityKey * speedMod * (new Vector3(X, 0.0f, Z)).normalized;
        
        // -- Translate the Player based on the view direction of the camera.
        gameObject.transform.position += new Vector3( Mathf.Cos(-angle) * direction.x - Mathf.Sin(-angle) * direction.z,
                                                      0.0f,
                                                      Mathf.Sin(-angle) * direction.x + Mathf.Cos(-angle) * direction.z);

        cameraObj.transform.position = new Vector3(cameraObj.transform.position.x, height, cameraObj.transform.position.z);
    }


    private void moveCamera() {
        float x = mouseDelta.y;

        if (cameraObj.transform.eulerAngles.x + mouseDelta.y > 70.0f && cameraObj.transform.eulerAngles.x + mouseDelta.y < 290.0f) {
            x = 0.0f;
        }

        cameraObj.transform.eulerAngles += new Vector3(x, -mouseDelta.x, 0.0f);
    }

  
    private void checkPlayerStamina() {
        // -- Change to a slow state.
        if (playerStats.getPlayerStamina() < 0.1f){
            setMovementState(MOVEMENTSTATE.SLOWED);
        }

        // -- Return to normal speed.
        else if (movementState == MOVEMENTSTATE.SLOWED && playerStats.getPlayerStamina() > 25.0f) {
            setMovementState(MOVEMENTSTATE.WALK);
        }
    }

}
