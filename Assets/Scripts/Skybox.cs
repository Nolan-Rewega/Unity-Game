using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{

    private GameObject player;

    void Start(){
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update(){
        gameObject.transform.position = player.transform.position;
    }
}
