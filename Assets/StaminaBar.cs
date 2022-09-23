using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    private Slider staminaFill;
    private PlayerStats playerStats;


    void Start(){
        staminaFill = gameObject.GetComponent<Slider>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }


    void Update(){
        staminaFill.value = playerStats.getPlayerStamina();
    }
}