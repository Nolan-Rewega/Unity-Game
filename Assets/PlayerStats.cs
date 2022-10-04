using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float m_staminaCap;
    [SerializeField] private float m_sanityRate;
    [SerializeField] private float m_sanityCap;
    [SerializeField] private float m_sanity;


    private float m_staminaUsageRate;
    private float m_staminaRegenRate;
    private float m_staminaAmount;
    private float m_stealthLevel;

    private bool m_inDarkness;
    private bool m_isRunning;
   

    void Start(){
        m_stealthLevel = 2.0f;

        m_staminaUsageRate = -18.0f;
        m_staminaRegenRate = 10.0f;
        m_staminaAmount = 100.0f;

        m_inDarkness = false;
        m_isRunning = false;
    }

    void Update(){

        m_sanity += (m_inDarkness) ? m_sanityRate : m_sanityRate * (-0.75f);
        m_sanity  = Mathf.Clamp(m_sanity, 0.0f, m_sanityCap);

        m_staminaAmount += (m_isRunning) ? m_staminaUsageRate * Time.deltaTime : m_staminaRegenRate * Time.deltaTime;
        m_staminaAmount  = Mathf.Clamp(m_staminaAmount, 0.0f, m_staminaCap);
    }

    // -- Get methods
    public float getPlayerStamina() {
        return m_staminaAmount;
    }
    public float getStaminaUsage() {
        return m_staminaUsageRate;
    }
    public float getStaminaRegen(){
        return m_staminaRegenRate;
    }
    public float getPlayerSanity(){
        return m_sanity;
    }
    public float getPlayerStealthLevel(){
        return m_stealthLevel;
    }


    // -- Set methods
    public void setStealthLevel(float value) {
        m_stealthLevel = value;
    }

    public void setRunningState(bool running) {
        m_isRunning = running;
    }

    public void setPlayerDarkness(bool inDarkness) {
        m_inDarkness = inDarkness;
    }

}
