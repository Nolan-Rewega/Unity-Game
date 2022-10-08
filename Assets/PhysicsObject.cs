using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour, SelectableInterface
{

    public void onSelection(Vector3 playerPos) {
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 2.0f) { return; }

        // -- Tell the player we are now holding this Object.
        GameObject.Find("Player").GetComponent<PlayerMovement>().updateHeldObject(gameObject);
    
    }

}
