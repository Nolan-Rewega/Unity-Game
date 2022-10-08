using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayCastSelection : MonoBehaviour
{
    [SerializeField] private GameObject rayPrefab;

    private Ray[] rayPool = new Ray[5];
    private float raySpeed;
    private int currentRay;

    private GameObject playerCamera;


    void Start(){
        playerCamera = gameObject.transform.GetChild(0).gameObject;
        raySpeed = 2500.0f;

        // -- Set up the ray pool.
        for (int i = 0; i < 5; i++){
            GameObject ray = Instantiate(rayPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            rayPool[i] = ray.GetComponent<Ray>();
            rayPool[i].gameObject.SetActive(false);
        }

        currentRay = 0;
    }


    public void castRay() {

        Quaternion rayRotation  = playerCamera.transform.rotation;
        Vector3    rayPosition  = playerCamera.transform.position;
        Vector3    rayDirection = playerCamera.transform.forward;
        
        rayPool[currentRay].gameObject.SetActive(true);
        rayPool[currentRay].fireRay(rayPosition, rayRotation, rayDirection, raySpeed);

        currentRay = (++currentRay) % 5;
    }

}
