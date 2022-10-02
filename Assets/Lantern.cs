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
    private GameObject playerCamera;
    private HDAdditionalLightData lightComponent;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerIntesity;
    private float flickerDuration;

    void Start(){
        player = GameObject.Find("Player");
        playerCamera = GameObject.Find("Player Camera");
        lightComponent = gameObject.transform.GetChild(0).GetComponent<HDAdditionalLightData>();

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

            if (lightComponent.enabled) {
                matches--;
            }

            lightComponent.intensity = (lightComponent.enabled) ? 0.8f : 0.0f;
        }

        toggleLight();
    }

    // -- CollectableInterface methods
    public void use(){
        // -- Play turn on sounds and animation.
        lightComponent.enabled = !lightComponent.enabled;
        if (lightComponent.enabled){
            matches--;
        }
    }

    public void action(){
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

        // -- While on, move the light infront of player, and flicker.
        //gameObject.transform.eulerAngles = playerCamera.transform.eulerAngles + new Vector3(180.0f, 0.0f, 0.0f);
        gameObject.transform.position = (playerCamera.transform.forward * 0.4f) + player.transform.position + (playerCamera.transform.right.normalized * -0.3f);

        lightComponent.intensity = Mathf.Lerp(flickerIntesity, 0.8f, t) * 500.0f;
    }


}
