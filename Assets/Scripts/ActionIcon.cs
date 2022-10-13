using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIcon : MonoBehaviour
{

    [SerializeField] private float selectionAngle;

    private void OnTriggerStay(Collider collision) {

        if (collision.name == "Player Camera") {
            // -- Do distance calculations
            Vector3 A = collision.transform.forward.normalized;
            Vector3 B = (gameObject.transform.position - collision.transform.position ).normalized;

            float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(A, B));

            if (angle < selectionAngle) {
                // -- Display interactable icon

                // -- SET NEAR OBJECT
                Debug.Log("TEST");
            }

        }
    }
}
