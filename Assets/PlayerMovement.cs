using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float sprintModifier;

    private Vector3 previousPos;

    private float sensitivityMouse;
    private float sensitivityKey;


    void Start(){
        previousPos = Input.mousePosition;
        sensitivityMouse = 0.5f;
        sensitivityKey   = 0.01f;
    }


    void Update(){
        // -- slight vertical bob, function z rotation left to right based on sin(delta.Time * pi).
        
        // -- Camera rotatio based on mouse movement.
        Vector3 currentPos = Input.mousePosition;
        Vector3 mouseDelta = (previousPos - currentPos) * sensitivityMouse;


        // -- Player movement.  W = +z, S = -Z, A = -x, D = +x;
        float sprint = (Input.GetKey(KeyCode.LeftShift)) ? 1.0f * sprintModifier : 1.0f;

        float Z = 0, X = 0;
        Z += (Input.GetKey(KeyCode.W)) ? 1.0f * sensitivityKey * sprint : 0.0f;
        Z -= (Input.GetKey(KeyCode.S)) ? 1.0f * sensitivityKey * sprint : 0.0f;
        X += (Input.GetKey(KeyCode.D)) ? 1.0f * sensitivityKey * sprint : 0.0f;
        X -= (Input.GetKey(KeyCode.A)) ? 1.0f * sensitivityKey * sprint : 0.0f;


        float angle = gameObject.transform.eulerAngles.y * Mathf.Deg2Rad;
        gameObject.transform.eulerAngles += new Vector3(mouseDelta.y, -mouseDelta.x, 0.0f);
        gameObject.transform.position    += new Vector3( Mathf.Cos(-angle) * X - Mathf.Sin(-angle) * Z, 0.0f, Mathf.Sin(-angle) * X + Mathf.Cos(-angle) * Z );

        // -- Switch to pub-sub broadcast method.
        // on e press cast a ray.

        previousPos = currentPos;
    }


}
