using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public void action() {
        // -- Tell the player what door to use
        GameObject.Find("Player").GetComponent<PlayerMovement>().updateDoorMode(gameObject);
    }

    public void rotateDoor(Vector3 playerDir, Vector3 mouseDir) {

        // -- 1. Transform playerDir and mouseDir into world space
        float phi = Mathf.Acos(Vector3.Dot(playerDir.normalized, Vector3.forward));
        Vector3 mouseDirTrans = new Vector3( Mathf.Cos(phi) * mouseDir.x - Mathf.Sin(phi) * mouseDir.z,
                                             0.0f,
                                             Mathf.Sin(phi) * mouseDir.x + Mathf.Cos(phi) * mouseDir.z );
        //Debug.Log(mouseDir);
        //Debug.Log(mouseDirTrans);


        // -- 2. Find the doors perpendicular vector
        Vector3 doorDir = gameObject.transform.right;
        float angle = 90.0f * Mathf.Deg2Rad;
        Vector3 perpendicular = new Vector3( Mathf.Cos(angle) * doorDir.x - Mathf.Sin(angle) * doorDir.z,
                                             0.0f,
                                             Mathf.Sin(angle) * doorDir.x + Mathf.Cos(angle) * doorDir.z );
        //Debug.Log(perpendicular);

        // -- 3. 
        //Debug.Log(doorDir);
        //Debug.Log(perpendicular.normalized);

        float rho = Mathf.Acos(Vector3.Dot(perpendicular.normalized, mouseDirTrans.normalized));

        //rho = (rho > Mathf.PI / 2.0f) ? 90.0f: rho;
        //Debug.Log(rho * Mathf.Rad2Deg);
        Vector3 old = gameObject.transform.eulerAngles;
        float value = (1.0f ) * (1.0f - (rho / (90.0f * Mathf.Deg2Rad)));

        old.y += value;
        gameObject.transform.eulerAngles = old;

    }

}
