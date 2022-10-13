using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnegryBar : MonoBehaviour{

    private Slider energyFill;
    private Flashlight flashlight;


    void Start(){
        energyFill = gameObject.GetComponent<Slider>();
        flashlight = GameObject.Find("Flashlight").GetComponent<Flashlight>();
    }


    void Update(){
        energyFill.value = flashlight.getEnergy();
    }
}
