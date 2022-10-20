using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCamera : MonoBehaviour
{

    void Update(){
        // -- Get the mouse delta.
        Vector3 mouseDelta = 1.0f * new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);
        float dirX = mouseDelta.y;
        float dirY = -mouseDelta.x;

        // -- Limit the directions the player can look.
        if (gameObject.transform.eulerAngles.x + dirX > 50.0f && gameObject.transform.eulerAngles.x + dirX < 310.0f){
            dirX = 0.0f;
        }
        if (gameObject.transform.eulerAngles.y + dirY > 60.0f && gameObject.transform.eulerAngles.y + dirY < 300.0f){
            dirY = 0.0f;
        }

        gameObject.transform.eulerAngles += new Vector3(dirX, dirY, 0.0f);

    }
}
