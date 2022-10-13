using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


public class Lantern : MonoBehaviour, SelectableInterface, UsableItemInterface, EquipableItemInterface, PlayerLightSource
{
    // [SerializeField] private float extinguishTime;
    [SerializeField] private ItemData referenceData;

    private bool isEquiped;
    private bool pickedUp;
    private int matchCounter;


    // -- Unity Objects and components
    private GameObject playerCamera;

    private Light lightComponent;
    private HDAdditionalLightData lighting;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerIntesity;
    private float flickerDuration;


    void Start(){ 
        playerCamera = GameObject.Find("Player Camera");

        lighting       = gameObject.transform.GetChild(0).GetComponent<HDAdditionalLightData>();
        lightComponent = gameObject.transform.GetChild(0).GetComponent<Light>();
        
        lightComponent.enabled = true;

        isEquiped = false;
        pickedUp = false;
        matchCounter = 3;
    }


    void Update(){
        // -- Do nothing until the player picks up the Lantern.
        if (!pickedUp) { return; }

        if (Input.GetKeyDown(KeyCode.F) && isEquiped) {
            // -- Check to see if the player has matches
            UsableItemInterface item = InventoryManager.Entity.searchItemByID("3");
            // -- If there is no item, then stack size is 0.
            int matches = (item != null) ? InventoryManager.Entity.getStackSizeByID("3") : 0;

            Debug.Log("The Player has: " + matches + " matches!");
            lightComponent.enabled = !lightComponent.enabled;

            if (lightComponent.enabled) {
                if (matches > 0){
                    // -- Only consume every third match. (SCUFFED CODE)
                    if (matchCounter == 1) {
                        ((Matches)item).consumeMatch();
                        matchCounter = 3;
                    }
                    else{ matchCounter--; }
                }
                else { lightComponent.enabled = false; }
            }

            lighting.intensity  = (lightComponent.enabled) ? 0.8f : 0.0f;
        }

        toggleLight();

    }

    // -- PlayerLightSource methods
    public bool getIsLightSourceOn() {
        return lightComponent.enabled;
    }

    // -- UsableItemInterface methods
    public void use(){
        // -- Play turn on sounds and animation.
        EquipableItemInterface src = (!isEquiped) ? this : null;
        EquipableManager.Entity.setEquipedItem(src);
    }

    // -- EquipableItemInterface methods
    public void equip(){ 
        // -- Play equip sounds and animation.
        isEquiped = true;
        lightComponent.enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public void unequip(){ 
        // -- Play unequip sounds and animation.
        isEquiped = false;
        lightComponent.enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public ItemData getItemData(){
        return referenceData;
    }


    // -- SelectableInterface methods
    public void onSelection(Vector3 playerPos){
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 2.0f) { return; }

        pickedUp = true;
        lightComponent.enabled = false;

        InventoryManager.Entity.add(this);

        // -- Disable mesh collider.
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }



    private void toggleLight(){

        // -- Update flashlights position
        Vector3 shiftedPosition = playerCamera.transform.position + new Vector3(0.2f, -0.2f, 0.2f);
        gameObject.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * 0.4f)
                                        + (playerCamera.transform.up * -0.2f) + (playerCamera.transform.right * 0.2f);
        gameObject.transform.rotation = playerCamera.transform.rotation;


        // -- If light is off do nothing.
        if (!lightComponent.enabled) { return; }

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / flickerDuration;

        if (t > 1.0f){
            flickerIntesity = Random.Range(0.4f, 0.8f);
            flickerDuration = Random.Range(0.1f, 0.3f);
            elapsedTime = 0.0f;
        }

        lighting.intensity = Mathf.Lerp(flickerIntesity, 0.8f, t) * 500.0f;
    }


}
