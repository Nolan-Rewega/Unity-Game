using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    public void action() {
        // -- turn on object hold
        GameObject.Find("Player").GetComponent<PlayerMovement>().updateHeldObject(gameObject);
    }

}
