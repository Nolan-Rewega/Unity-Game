using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    [SerializeField] private float rayLifeTime;
    private float totalTime;
    private Vector3 rayStartPosition;

    private Rigidbody rb;

    void Awake() {
        totalTime = 0.0f;
        rayStartPosition = gameObject.transform.position;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update() {
        totalTime += Time.deltaTime;

        if (totalTime > rayLifeTime) {
            gameObject.SetActive(false);
            totalTime = 0.0f;
        }

    }

    public void fireRay(Vector3 position, Quaternion rotation, Vector3 direction, float speed) {
        // -- Each Ray is usally fired from the player camera's position.
        rayStartPosition = position;

        gameObject.transform.SetPositionAndRotation(position, rotation);
        rb.velocity = speed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter(Collider other) {
        // -- Do nothing on player collision.
        if (other.gameObject.name == "Player") { return; }


        if (other.gameObject.layer == 6){  // -- 6 = Selectable.
            // -- Selectable Object must have a SelectableInterface Component.
            var monoBehaviourObjects = other.gameObject.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour mb in monoBehaviourObjects) {
                var interfaceType = mb.GetType().GetInterface("SelectableInterface");

                if (interfaceType != null){
                    ((SelectableInterface)mb).onSelection(rayStartPosition);
                }

            }
        }
        else {
            gameObject.SetActive(false);
        }
    }
}
