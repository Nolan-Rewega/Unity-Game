using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HidingSpaceBase : MonoBehaviour
{
    private PlayerStats playerStats;
    private float previousStealthLevel;

    void Start() {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        previousStealthLevel = 0.0f;
    }



    public void OnTriggerEnter(Collider collider) {
        // -- Increase stealth value.
        previousStealthLevel = playerStats.getPlayerStealthLevel();
        playerStats.setStealthLevel(0.1f);
        Debug.Log("Hiding space entered");
    }

    public void OnTriggerExit(Collider collider) {
        // -- Decrease Stealth value.
        playerStats.setStealthLevel(previousStealthLevel);
        Debug.Log("Hiding space entered");
    }

    
}
