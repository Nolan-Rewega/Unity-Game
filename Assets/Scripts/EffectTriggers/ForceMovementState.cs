using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceMovementState : MonoBehaviour
{
    [SerializeField] private PlayerMovement.MOVEMENTSTATE state;
    private PlayerMovement playerMovement;

    void Start(){
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public void OnTriggerEnter(Collider collider){
        // -- Force the player into "state" and prevent them from leaving it
        playerMovement.haltMovementStateChange(true);
        playerMovement.setMovementState(state);
    }

    public void OnTriggerExit(Collider collider){
        playerMovement.haltMovementStateChange(false);
    } 
}
