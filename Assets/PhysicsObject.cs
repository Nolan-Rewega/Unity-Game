using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    public void action() {
        // -- Tell the player we are now holding this Object.
        GameObject.Find("Player").GetComponent<PlayerMovement>().updateHeldObject(gameObject);
    }

}
