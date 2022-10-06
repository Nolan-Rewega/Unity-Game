using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayCastSelection : MonoBehaviour
{
    [SerializeField] private GameObject rayPrefab;

    private GameObject[] rayPool = new GameObject[5];
    private float raySpeed;
    private int currentRay;

    private float pickUpRange;

    private GameObject playerCamera;


    void Start(){
        playerCamera = gameObject.transform.GetChild(0).gameObject;

        raySpeed = 25.0f;
        pickUpRange = 1.8f;

        // -- Set up the ray pool.
        for (int i = 0; i < 5; i++){
            rayPool[i] = Instantiate(rayPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            rayPool[i].SetActive(false);
        }

        currentRay = 0;
    }



    public void castRay() {
        Vector3 dir = playerCamera.transform.eulerAngles;

        Quaternion rayRotation  =  Quaternion.Euler(dir.x, dir.y, dir.z);
        Vector3    rayPosition  = playerCamera.transform.position;

        GameObject ray = rayPool[currentRay];
        ray.SetActive(true);
        ray.transform.SetPositionAndRotation(rayPosition, rayRotation);

        Rigidbody rayBody = ray.GetComponent<Rigidbody>();
        rayBody.velocity  = playerCamera.transform.forward * raySpeed;

        currentRay = (++currentRay) % 5;
    }


    public void rayCastCallback(GameObject hitObject) {
        
        // -- Enforce maximum pick up range
        float distance = (hitObject.transform.position - gameObject.transform.position).magnitude;

        // -- I need a better wau
        if (distance > pickUpRange) { return; }
        if (hitObject.tag == "Battery") {
            hitObject.GetComponent<Battery>().onPickUp();
        }
        else if (hitObject.tag == "Flashlight") {
            hitObject.GetComponent<Flashlight>().onPickUp();
        }
        else if (hitObject.tag == "Lantern") {
            hitObject.GetComponent<Lantern>().onPickUp();

        }

        if (distance > 3.0f) { return; }
        else if (hitObject.tag == "Door"){
            // -- Enter Interaction mode on press.
            hitObject.GetComponent<Door>().action();
        }

        if (distance > 2.0f) { return; }
        else if (hitObject.tag == "Physics"){
            // -- Enter enter carry mode on press
            hitObject.GetComponent<PhysicsObject>().action();
        }

    }
}
