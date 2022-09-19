using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private bool toggled;

    // -- GameObjects and Components 
    private GameObject camera;
    private Light lightComponent;
    
    // -- flashlight parameters
    private float maxIntensity;


    public float energyDrainRate;
    public float energyCap;
    public float energy;

    void Start(){
        camera = GameObject.Find("Player Camera");
        lightComponent = gameObject.GetComponent<Light>();

        toggled = false;
        maxIntensity = 1.0f;
        lightComponent.intensity = 0.0f;
    }


    void Update(){
        Vector3 cameraPosition = camera.transform.position;
        Vector3 cameraAngle    = camera.transform.eulerAngles;

        // -- Update flashlights position
        gameObject.transform.position = camera.transform.position;
        gameObject.transform.eulerAngles = new Vector3(cameraAngle.x, cameraAngle.y, 0.0f);


        // -- battery drainage.
        if (toggled) {
            energy -= energyDrainRate * Time.deltaTime;
            float flickerRate = energy / energyCap;

            if (energy <= 0.0f){
                // -- play fizzle sound.
                toggled = false;
                lightComponent.intensity = 0.0f;
            }
            else {
                lightComponent.intensity = Random.Range(maxIntensity * flickerRate, maxIntensity);
            }
           
        }




        if (Input.GetKeyDown(KeyCode.F)) {
            // -- Play click sound.
            if(energy > 0.0f) {
                toggled = !toggled;
                lightComponent.intensity = (toggled) ? maxIntensity : 0.0f;
            }
        }
    }
}
