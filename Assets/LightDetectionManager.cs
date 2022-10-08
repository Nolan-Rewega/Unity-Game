using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetectionManager : MonoBehaviour
{
    public static LightDetectionManager Entity;

    private PlayerLightSource currentLightSource;
    private PlayerStats playerStats;
    private List<GameObject> lightsSources;


    // Start is called before the first frame update
    void Start(){
        Entity = this;
        currentLightSource = null;

        lightsSources = new List<GameObject>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }


    public void addLight(GameObject light) {
        if (lightsSources.Contains(light)){
            Debug.Log("Player already in light.");
            return;
        }

        lightsSources.Add(light);
        updateInDarkness();
    }

    public void removeLight(GameObject light) {
        if (!lightsSources.Contains(light)) {
            Debug.Log("Player not in light.");
            return;
        }
        lightsSources.Remove(light);
        updateInDarkness();

    }

    public void setPlayerLightSource(PlayerLightSource light) {
        // -- Equip the new light source.
        if (light != null){ 
            light.equip(); 
        }

        // -- Unequip previous light source.
        if (currentLightSource != null) {
            currentLightSource.unequip();
        }

        // -- Change the current light source
        currentLightSource = light;
    }

    public bool getIsPlayerLightSourceOn() {
        if (currentLightSource == null) { return false; }

        return currentLightSource.getIsLightSourceOn();
    }


    private void updateInDarkness() {
        playerStats.setPlayerInDarkness(lightsSources.Count == 0);
    }
}
