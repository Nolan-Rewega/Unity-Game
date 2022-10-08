using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetectionTrigger : MonoBehaviour
{

    // -- Might have to change to slow polling.
    private void OnTriggerEnter(Collider other) {
        LightDetectionManager.Entity.addLight(gameObject);
    }


    void OnTriggerExit(Collider other) {
        LightDetectionManager.Entity.removeLight(gameObject);
    }
}
