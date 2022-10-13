using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetectionManager : MonoBehaviour
{
    public static LightDetectionManager Entity;

    private PlayerStats playerStats;
    private List<GameObject> lightsSources;


    // Start is called before the first frame update
    void Start(){
        Entity = this;

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

    private void updateInDarkness() {
        playerStats.setPlayerInDarkness(lightsSources.Count == 0);
    }
}
