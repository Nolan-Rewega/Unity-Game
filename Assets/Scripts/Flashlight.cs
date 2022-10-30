using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Flashlight : MonoBehaviour, SelectableInterface, EquipableItemInterface, UsableItemInterface, PlayerLightSource
{
    [SerializeField] private ItemData referenceData;
    [SerializeField] private float energyDrainRate;
    [SerializeField] private float energyCap;
    [SerializeField] private float energy;

    private bool isEquiped;
    private bool pickedUp;

    // -- GameObjects and Components 
    private GameObject playerCamera;
    private Light lightComponent;
    private HDAdditionalLightData lighting;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerDuration;
    private float flickerIntensity;
    private float currUpperLimit;

    void Start() {
        playerCamera = GameObject.Find("Player Camera");

        lighting = gameObject.GetComponent<HDAdditionalLightData>();
        lightComponent = gameObject.GetComponent<Light>();

        isEquiped = false;
        pickedUp  = false;

        lightComponent.enabled = false;
        lighting.intensity = 0.0f;

        // -- Initial flicker intensity and duration.
        elapsedTime    = 0.0f;
        currUpperLimit = 1.0f;
        flickerIntensity = 1.0f;
        flickerDuration  = 0.0f;
    }


    void Update() {
        // -- Do nothing until the player picks up the Lantern.
        if (!pickedUp) { return; }


        if (Input.GetKeyDown(KeyCode.F) && isEquiped) {
            // -- Play turn on sounds and animation.
            lightComponent.enabled = !lightComponent.enabled;
        }

        toggleLight();
    }


    public float getEnergy() {
        return energy;
    }

    public void increaseEnergy(float value) {
        energy = Mathf.Min(energy + value, energyCap);
    }


    private void toggleLight() {

        // -- Update flashlights position
        Vector3 shiftedPosition = playerCamera.transform.position + new Vector3(0.2f, -0.2f, 0.2f);
        gameObject.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * 0.4f)
                                      + (playerCamera.transform.up * -0.2f) + (playerCamera.transform.right * 0.2f);
        gameObject.transform.rotation = playerCamera.transform.rotation;


        // -- If light is off do nothing.
        if (!lightComponent.enabled ) { return; }

        // -- Calculate upper intensity limit
        float upperLimit = Mathf.Clamp((energy / energyCap + 0.3f), 0.4f, 1.0f);
        float lowerLimit = upperLimit - (0.4f * Mathf.Clamp((1.0f - (energy / energyCap + 0.2f)), 0.0f, 1.0f));

        // -- Drain Battery.
        energy -= energyDrainRate * Time.deltaTime;

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / flickerDuration;

        if (energy <= 0.0f){
            // -- play fizzle sound. and animation
            lighting.intensity = 0.0f;
        }
        else{
            if (t > 1.0f){
                elapsedTime    = 0.0f;
                currUpperLimit = upperLimit;
                flickerIntensity = Random.Range(lowerLimit, upperLimit);
                flickerDuration  = Random.Range(0.1f, 0.5f * (energy / energyCap + 0.1f));
            }

            lighting.intensity = Mathf.Lerp(flickerIntensity, currUpperLimit, t) * 500.0f;
        }

    }


    // -- PlayerLightSource methods
    public bool getIsLightSourceOn(){
        return lightComponent.enabled;
    }

    // -- UsableItemInterface methods
    public void equip() { // called by LightDetectionManager
        // -- Play equip sounds and animation.
        isEquiped = true;
        lightComponent.enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public void unequip() { // called by LightDetectionManager
        // -- Play unequip sounds and animation.
        isEquiped = false;
        lightComponent.enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
    public void use() {
        // -- Play turn on sounds and animation.
        EquipableItemInterface src = (!isEquiped) ? this : null;
        EquipableManager.Entity.setEquipedItem(src);
    }
    public ItemData getItemData(){
        return referenceData;
    }


    // -- SelectableInterface methods
    public void onSelection(Vector3 playerPos) {
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 2.0f) { return; }


        pickedUp = true;
        InventoryManager.Entity.add(this);

        // -- Disable mesh collider.
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false; 
    }



}
