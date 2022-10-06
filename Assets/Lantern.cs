using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


public class Lantern : MonoBehaviour, SelectableInterface
{
    // [SerializeField] private float extinguishTime;
    [SerializeField] private ItemData referenceData;

    private bool hasLantern;
    private int matches;
    
    // -- Unity Objects and components
    private GameObject player;
    private PlayerMovement playerMovement;
    private GameObject playerCamera;

    private Light lightComponent;
    private HDAdditionalLightData lighting;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerIntesity;
    private float flickerDuration;

    void Start(){
        player = GameObject.Find("Player");
        playerCamera = GameObject.Find("Player Camera");
        playerMovement = player.GetComponent<PlayerMovement>();

        lighting       = gameObject.transform.GetChild(0).GetComponent<HDAdditionalLightData>();
        lightComponent = gameObject.transform.GetChild(0).GetComponent<Light>();
        
        matches = 10;
        lightComponent.enabled = false;
        hasLantern = false;

        // -- Generate initial flicker intensity and duration.
        elapsedTime = 0.0f;
        flickerIntesity = 0.8f;
        flickerDuration = 0.5f;
    }


    void Update(){

        if (Input.GetKeyDown(KeyCode.F) && hasLantern && matches > 0){
            lightComponent.enabled = !lightComponent.enabled;
            playerMovement.setLightSrcOn(lightComponent.enabled);

            if (lightComponent.enabled) {
                matches--;
            }

            lighting.intensity = (lightComponent.enabled) ? 0.8f : 0.0f;
        }

        toggleLight();
        if (hasLantern) {
            // -- Update flashlights position
            Vector3 shiftedPosition = playerCamera.transform.position + new Vector3(0.2f, -0.2f, 0.2f);
            gameObject.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * 0.4f)
                                            + (playerCamera.transform.up * -0.2f) + (playerCamera.transform.right * 0.2f);
            gameObject.transform.rotation = playerCamera.transform.rotation;
        }

    }

    // -- CollectableInterface methods
    public void use(){
        // -- Play turn on sounds and animation.
        lightComponent.enabled = !lightComponent.enabled;
        playerMovement.setLightSrcOn(lightComponent.enabled);
        if (lightComponent.enabled){
            matches--;
        }
    }

    public void onPickUp(){
        // -- On Item pickup
        hasLantern = true;
        InventorySystem.Entity.add(this);

        // -- Disable mesh collider.
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public ItemData getItemData(){
        return referenceData;
    }


    private void toggleLight(){
        if (!lightComponent.enabled || matches <= 0) { return; }

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
