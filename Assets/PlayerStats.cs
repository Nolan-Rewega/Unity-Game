using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float m_sanityRate;
    [SerializeField] private float m_sanity;

    private float m_staminaUsageRate;
    private float m_staminaRegenRate;
    private float m_staminaAmount;
    private float m_stealthLevel;

    private bool m_inDarkness;
    private bool m_isRunning;
   

    void Start(){
        setPlayerStats(10.0f, -18.0f, 2.0f);

        m_staminaAmount = 100.0f;
        m_inDarkness = false;
        m_isRunning = false;
    }

    void Update(){

        m_sanity -= (m_inDarkness) ? m_sanityRate : m_sanityRate * (-0.75f);
        m_sanity  = Mathf.Clamp(m_sanity, 0.0f, 1.0f);

        m_staminaAmount += (m_isRunning) ? m_staminaUsageRate * Time.deltaTime : m_staminaRegenRate * Time.deltaTime;
        m_staminaAmount  = Mathf.Clamp(m_staminaAmount, 0.0f, 100.0f);
    }

    // -- Called when the player switches movement states.
    public void setMovementState(int state) {
        switch (state) {
            case 0: // -- Walk
                setPlayerStats(10.0f, -18.0f, 2.0f);
                break;

            case 1: // -- Crouch
                setPlayerStats(13.5f, -15.0f, 1.4f);
                break;

            case 2: // -- Prone
                setPlayerStats(20.0f, -12.0f, 0.5f);
                break;

            default:
                break;
        }
    }
    public void setRunningState(bool running) {
        m_isRunning = running;
    }

    public float getPlayerSanity() {
        return m_sanity;
    }

    public void setPlayerDarkness(bool inDarkness) {
        m_inDarkness = inDarkness;
    }

    public float getPlayerStamina() {
        return m_staminaAmount;
    }

    public float getPlayerStealthLevel() {
        return (m_isRunning) ? m_stealthLevel * 2.0f : m_stealthLevel;
    }


    // -- Helper method.
    private void setPlayerStats( float staminaRegen, float staminaUsage, float stealthLevel ){
        m_staminaRegenRate = staminaRegen;
        m_staminaUsageRate = staminaUsage;
        m_stealthLevel = stealthLevel;
    }

}
