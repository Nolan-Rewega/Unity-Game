using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // -- Private attributes.
    private enum MOVEMENTSTATE { WALK, CROUCH, PRONE, RUN };

    private MOVEMENTSTATE movementState;

    private GameObject cameraObj;
    private PlayerStats playerStats;

    private Vector3 mouseDelta;

    private float currMovementModifier;
    private float prevMovementModifier;

    private float interpolatedHeight;
    private float interpolatedSpeed;

    private bool haltedPlayerMovement;
    private bool haltedPlayerCamera;

    private float sensitivityMouse;
    private float sensitivityKey;
    private float currCameraY;
    private float prevCameraY;

    private bool interpolating;
    private float currDuration;
    private float elapsedTime;

    private float idleTime;

    private bool outOfBreath;


    void Start(){
        movementState    = MOVEMENTSTATE.WALK;

        haltedPlayerMovement = false;
        haltedPlayerCamera = false;

        currMovementModifier = 1.0f;
        prevMovementModifier = 1.0f;
        sensitivityMouse =  3.0f;
        sensitivityKey   =  0.01f;

        currCameraY   = 1.0f;
        prevCameraY   = 1.0f;

        idleTime    = 0.0f;
        elapsedTime = 0.0f;

        playerStats = gameObject.GetComponent<PlayerStats>();
        cameraObj   = gameObject.transform.GetChild(0).gameObject;
    }


    void Update(){
        mouseDelta = sensitivityMouse * new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);

        checkStealthMode();
        checkPlayerStamina();
        modifyPlayerMovement();

        if (!haltedPlayerMovement) { movePlayer(); }
        if (!haltedPlayerCamera) { moveCamera(); }
    }


    public void haltPlayerMovement(bool value) {
        haltedPlayerMovement = value;
    }
    public void haltPlayerCamera(bool value) {
        haltedPlayerCamera = value;
    }


    private void modifyPlayerMovement() {
        // -- Cannot change state while out of breath.
        if (outOfBreath) { return; }

        // -- Change movement state based on key input.
        //    TODO: Expand to full state machine.
        if (( Input.GetKeyDown(KeyCode.LeftControl) && movementState == MOVEMENTSTATE.CROUCH) ||
            ( Input.GetKeyDown(KeyCode.C)           && movementState == MOVEMENTSTATE.PRONE )    ){
            setMovementState(MOVEMENTSTATE.WALK, 1.0f, 1.5f, 0.2f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) {
            setMovementState(MOVEMENTSTATE.RUN, 1.0f, 3.0f, 0.2f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && movementState == MOVEMENTSTATE.RUN || 
                 Input.GetKeyUp(KeyCode.W)         && movementState == MOVEMENTSTATE.RUN    ) {
            setMovementState(MOVEMENTSTATE.WALK, 1.0f, 1.5f, 0.2f);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl)){
            setMovementState(MOVEMENTSTATE.CROUCH, 0.5f, 0.7f, 0.2f);
        }
        else if (Input.GetKeyDown(KeyCode.C)){
            setMovementState(MOVEMENTSTATE.PRONE, 0.2f, 0.3f, 0.3f);
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

    private void checkStealthMode() {
        // -- Fail conditions.
        if (movementState != MOVEMENTSTATE.PRONE) {
            idleTime = 0.0f; // -- Reset StealthMode timer.
            return; 
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            idleTime = 0.0f; // -- Reset StealthMode timer.
            return; 
        }
        if (mouseDelta.sqrMagnitude > 0.0f) {
            idleTime = 0.0f; // -- Reset StealthMode timer.
            return; 
        }
        // -- Flashlight or Lantern must be off.
        if (EquipableManager.Entity.getIsPlayerLightSourceOn()) {
            idleTime = 0.0f;
            return;
        }

        idleTime += Time.deltaTime;
        float t = idleTime / 2.0f;

        if (t > 1.0f) {
            // -- In stealth mode. starts decreasing sanity. increase stealth level alot
            Debug.Log("STEALTH MODE. ");
            playerStats.setStealthLevel(0.1f);
        }
    }

    private void checkPlayerStamina() {
        if (playerStats.getPlayerStamina() < 0.1f){
            outOfBreath = true;

            // -- Change to a slow state.
            setMovementState(MOVEMENTSTATE.WALK, 1.0f, 0.4f, 0.3f);

        }
        else if (outOfBreath && playerStats.getPlayerStamina() > 25.0f) {
            // -- Return to normal speed.
            setMovementState(MOVEMENTSTATE.WALK, 1.0f, 1.5f, 0.2f);
            outOfBreath = false;
        }
    }


    private void setMovementState(MOVEMENTSTATE state, float height, float modifier, float lerpDuration) {
        // -- Tell playerStats the new MOVEMENTSTATE.
        if      (state == MOVEMENTSTATE.WALK)   { playerStats.setStealthLevel(2.0f); }
        else if (state == MOVEMENTSTATE.CROUCH) { playerStats.setStealthLevel(1.0f); }
        else if (state == MOVEMENTSTATE.PRONE)  { playerStats.setStealthLevel(0.4f); }
        else if (state == MOVEMENTSTATE.RUN)    { playerStats.setStealthLevel(4.0f); }

        // -- Tell playerStats if the player is running or not.
        if (state == MOVEMENTSTATE.RUN) { playerStats.setRunningState(true); } 
        else { playerStats.setRunningState(false); }

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
        currMovementModifier = modifier;
        currCameraY = height;

        interpolating = true;
        currDuration = lerpDuration;
    }
}
