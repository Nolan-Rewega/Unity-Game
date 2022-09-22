using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    [SerializeField] private float energyDrainRate;
    [SerializeField] private float energyCap;
    [SerializeField] private float energy;

    private bool toggled;

    // -- GameObjects and Components 
    private GameObject playerCamera;
    private Light lightComponent;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerDuration;
    private float flickerIntensity;
    private float currUpperLimit;

    void Start(){
        playerCamera = GameObject.Find("Player Camera");
        lightComponent = gameObject.GetComponent<Light>();

        toggled = false;
        lightComponent.intensity = 0.0f;
    }


    void Update(){
        // -- Calculate upper intensity limit
        float upperLimit = Mathf.Clamp((energy / energyCap + 0.3f), 0.4f, 1.0f);
        float lowerLimit = upperLimit - (0.4f * Mathf.Clamp((1.0f - (energy / energyCap + 0.2f)), 0.0f, 1.0f));

        //Debug.Log("UPPER: " +  upperLimit + " LOWER: " + lowerLimit);
        // -- Update flashlights position
        gameObject.transform.position = playerCamera.transform.position;
        gameObject.transform.eulerAngles = playerCamera.transform.eulerAngles;

        if (Input.GetKeyDown(KeyCode.F) && !toggled){
            // -- Play turn on sounds and animation.
            toggled = true;
            lightComponent.intensity = 1.0f;

            // -- Generate initial flicker intensity and duration.
            elapsedTime = 0.0f;
            currUpperLimit = upperLimit;
            flickerIntensity = Random.Range(lowerLimit, upperLimit);
            flickerDuration  = Random.Range(0.1f, 0.5f * (energy / energyCap + 0.1f));
        }
        else if (Input.GetKeyDown(KeyCode.F) && toggled) {
            // -- Play turn of sounds and animation.
            toggled = false;
            lightComponent.intensity = 0.0f;
        }



        // -- battery drainage.
        if (toggled) {
            energy -= energyDrainRate * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flickerDuration;

            if (energy <= 0.0f){
                // -- play fizzle sound.
                toggled = false;
                lightComponent.intensity = 0.0f;
            }
            else{
                if (t > 1.0f){
                    elapsedTime = 0.0f;
                    currUpperLimit = upperLimit;
                    flickerIntensity = Random.Range(lowerLimit, upperLimit);
                    flickerDuration = Random.Range(0.1f, 0.5f * (energy / energyCap + 0.1f));
                }

                lightComponent.intensity = Mathf.Lerp(flickerIntensity, currUpperLimit, t);
            }
        }
    }

    public float getEnergy() { 
        return energy; 
    }

    public void increaseEnergy(float value) {
        energy = Mathf.Min(energy + value, energyCap);
    }


}
