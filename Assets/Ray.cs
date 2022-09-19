using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    [SerializeField] private float rayLifeTime;
    private float totalTime;


    void Start() {
        totalTime = 0.0f;
    }

    void Update() {
        totalTime += Time.deltaTime;

        if (totalTime > rayLifeTime) {
            gameObject.SetActive(false);
            totalTime = 0.0f;
        }

    }


    private void OnTriggerEnter(Collider other) {
 
        // -- 6 = interactable
        if (other.gameObject.layer == 6 || other.gameObject.name == "Player Camera") { 
            GameObject.Find("Player Camera").GetComponent<RayCastSelection>().rayCastCallback(other.gameObject);
        }
        else {
            gameObject.SetActive(false);
        }
    }
}
