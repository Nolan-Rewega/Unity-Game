using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDisarm : MonoBehaviour
{

    public void OnTriggerEnter(Collider collider) {
        // -- Force the player into "state" and prevent them from leaving it
        EquipableManager.Entity.setForceDisarm(true);
        EquipableManager.Entity.setEquipedItem(null);
    }

    public void OnTriggerExit(Collider collider) {
        EquipableManager.Entity.setForceDisarm(false);
    } 
}
