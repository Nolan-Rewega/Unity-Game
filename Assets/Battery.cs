using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour, SelectableInterface
{
    [SerializeField] private ItemData referenceData;

    private Flashlight flashlight;

    void Start(){
        // -- might have to get changed.
        flashlight = GameObject.Find("Flashlight").GetComponent<Flashlight>();
    }

    public ItemData getItemData() {
        return referenceData;
    }

    public void action() {
        // -- Adds Battery to the players inventory
        InventorySystem.Entity.add(this);

        // -- Disable until use().
        gameObject.SetActive(false);
    }

    // -- On Inventory click or pressed Q
    public void use() {
        // -- Use the Item
        gameObject.SetActive(true);
        flashlight.increaseEnergy(100.0f);

        // -- Remove the battery from inventory.
        InventorySystem.Entity.remove(this);
        Destroy(gameObject);
    }

}
