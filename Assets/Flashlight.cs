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
    private PlayerMovement player;
    private GameObject playerCamera;

    private Light lightComponent;
    private HDAdditionalLightData lighting;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerDuration;
    private float flickerIntensity;
    private float currUpperLimit;

    void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        playerCamera = GameObject.Find("Player Camera");

        lighting = gameObject.GetComponent<HDAdditionalLightData>();
        lightComponent = gameObject.GetComponent<Light>();

        hasFlashlight = false;
        lightComponent.enabled = false;
        lighting.intensity = 0.0f;

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
            player.setLightSrcOn(lightComponent.enabled);
        }

        toggleLight();

        // -- Update flashlights position
        if (hasFlashlight) {
            Vector3 shiftedPosition = playerCamera.transform.position + new Vector3(0.2f, -0.2f, 0.2f);
            gameObject.transform.position = playerCamera.transform.position     + (playerCamera.transform.forward * 0.4f) 
                                          + (playerCamera.transform.up * -0.2f) + (playerCamera.transform.right   * 0.2f);
            gameObject.transform.rotation = playerCamera.transform.rotation;
        }

    }

    private void toggleLight() {
        
        if (!lightComponent.enabled ) { return; }

        // -- Calculate upper intensity limit
        float upperLimit = Mathf.Clamp((energy / energyCap + 0.3f), 0.4f, 1.0f);
        float lowerLimit = upperLimit - (0.4f * Mathf.Clamp((1.0f - (energy / energyCap + 0.2f)), 0.0f, 1.0f));

        // -- Drain Battery.
        energy -= energyDrainRate * Time.deltaTime;

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / flickerDuration;

        if (energy <= 0.0f){
            // -- play fizzle sound.
            lighting.intensity = 0.0f;
        }
        else{
            if (t > 1.0f){
                elapsedTime    = 0.0f;
                currUpperLimit = upperLimit;
                flickerIntensity = Random.Range(lowerLimit, upperLimit);
                flickerDuration  = Random.Range(0.1f, 0.5f * (energy / energyCap + 0.1f));
            }

            lighting.intensity = Mathf.Lerp(flickerIntensity, currUpperLimit, t) * 1200.0f;
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
        player.setLightSrcOn(lightComponent.enabled);
    }

    public void onPickUp() {
        // -- Add item to player's inventory.
        hasFlashlight = true;
        InventorySystem.Entity.add(this);

        // -- Disable mesh collider.
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    public ItemData getItemData() {
        return referenceData;
    }


}
