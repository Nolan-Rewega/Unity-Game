using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
   // [SerializeField] private float extinguishTime;

    private bool toggled;
    private int matches;
    
    // -- Unity Objects and components
    private GameObject player;
    private GameObject playerCamera;
    private Light lightComponent;

    // -- Interpolation properties.
    private float elapsedTime;
    private float flickerIntesity;
    private float flickerDuration;

    void Start(){
        player = GameObject.Find("Player");
        playerCamera = GameObject.Find("Player Camera");
        lightComponent = gameObject.GetComponent<Light>();

        matches = 10;
        toggled = false;
        lightComponent.intensity = 0.0f;
    }


    void Update(){

        if (Input.GetKeyDown(KeyCode.F) && !toggled && matches > 0){
            // -- Play match lighting animation.
            toggled = true;
            matches--;
            lightComponent.intensity = 0.8f;

            // -- Generate initial flicker intensity and duration.
            elapsedTime = 0.0f;
            flickerIntesity = Random.Range(0.2f, 0.8f);
            flickerDuration = Random.Range(0.1f, 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.F) && toggled) {
            // -- Play extinguish animation.
            toggled = false;
            lightComponent.intensity = 0.0f;
        }


        if (toggled) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flickerDuration;

            if (t > 1.0f ){
                flickerIntesity = Random.Range(0.4f, 0.8f);
                flickerDuration = Random.Range(0.1f, 0.3f);
                elapsedTime = 0.0f;
            }

            // -- While on, move the light infront of player, and flicker.
            gameObject.transform.position = (playerCamera.transform.forward * 0.4f) + player.transform.position + (playerCamera.transform.right.normalized * -0.3f);
            lightComponent.intensity = Mathf.Lerp(flickerIntesity, 0.8f, t);

        }
    }

}
