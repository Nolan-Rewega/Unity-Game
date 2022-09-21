using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{

    private GameObject flashlight;

    void Start(){
        flashlight = GameObject.Find("Flashlight");
    }

    public void action() {
        // -- Consume object to gain flashlight energy.
        // play noise.
        flashlight.GetComponent<Flashlight>().increaseEnergy(33.0f);
        Destroy(gameObject);
    }

}
