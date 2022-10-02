using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Flashlight : MonoBehaviour, SelectableInterface
{
    [SerializeField] private ItemData referenceData;
    [SerializeField] private float energyDrainRate;
    [SerializeField] private float energyCap;
    [SerializeField] private float energy;

    private bool hasFlashlight;

    // -- GameObjects and Components 
    private GameObject playerCamera;
    private HDAdditionalLightData lightComponent;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerDuration;
    private float flickerIntensity;
    private float currUpperLimit;

    void Start() {
        playerCamera = GameObject.Find("Player Camera");
        lightComponent = gameObject.GetComponent<HDAdditionalLightData>();

        hasFlashlight = false;
        lightComponent.enabled = false;

        // -- Initial flicker intensity and duration.
        elapsedTime    = 0.0f;
        currUpperLimit = 1.0f;
        flickerIntensity = 1.0f;
        flickerDuration  = 0.0f;
    }


    void Update() {

        if (Input.GetKeyDown(KeyCode.F) && hasFlashlight) {
            // -- Play turn on sounds and animation.
            lightComponent.enabled = !lightComponent.enabled;
        }

        toggleLight();

    }

    private void toggleLight() {
        
        if (!lightComponent.enabled ) { return; }

        // -- Calculate upper intensity limit
        float upperLimit = Mathf.Clamp((energy / energyCap + 0.3f), 0.4f, 1.0f);
        float lowerLimit = upperLimit - (0.4f * Mathf.Clamp((1.0f - (energy / energyCap + 0.2f)), 0.0f, 1.0f));

        // -- Update flashlights position
        gameObject.transform.position    = playerCamera.transform.position;
        gameObject.transform.eulerAngles = playerCamera.transform.eulerAngles;

        // -- Drain Battery.
        energy -= energyDrainRate * Time.deltaTime;

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / flickerDuration;

        if (energy <= 0.0f){
            // -- play fizzle sound.
            lightComponent.intensity = 0.0f;
        }
        else{
            if (t > 1.0f){
                elapsedTime    = 0.0f;
                currUpperLimit = upperLimit;
                flickerIntensity = Random.Range(lowerLimit, upperLimit);
                flickerDuration  = Random.Range(0.1f, 0.5f * (energy / energyCap + 0.1f));
            }

            lightComponent.intensity = Mathf.Lerp(flickerIntensity, currUpperLimit, t) * 1200.0f;
        }
  
    }

    public float getEnergy() {
        return energy;
    }

    public void increaseEnergy(float value) {
        energy = Mathf.Min(energy + value, energyCap);
    }


    // -- CollectableInterface methods
    public void use() {
        // -- Play turn on sounds and animation.
        lightComponent.enabled = !lightComponent.enabled;
    }

    public void action() {
        // -- On Item pickup
        hasFlashlight = true;
        InventorySystem.Entity.add(this);

        // -- Disable mesh collider.
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public ItemData getItemData() {
        return referenceData;
    }


}
