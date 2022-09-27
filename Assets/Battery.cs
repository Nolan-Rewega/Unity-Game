using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] private ItemData referenceData;

    private GameObject flashlight;
    private InventorySystem inventorySystem;

    void Start(){
        flashlight      = GameObject.Find("Flashlight");
        inventorySystem = GameObject.Find("Inventory").GetComponent<InventorySystem>();
    }


    public void action() {
        // -- Adds Battery to the players inventory
        flashlight.GetComponent<Flashlight>().increaseEnergy(33.0f);
        inventorySystem.add(referenceData);
        Destroy(gameObject);
    }

}
