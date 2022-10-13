using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour, SelectableInterface, UsableItemInterface
{
    [SerializeField] private ItemData referenceData;

    private Flashlight flashlight;

    void Start(){
        // -- might have to get changed.
        flashlight = GameObject.Find("Flashlight").GetComponent<Flashlight>();
    }



    // -- SelectableInterface methods
    public void onSelection(Vector3 playerPos) {
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 2.0f) { return; }

        // -- Adds Battery to the players inventory
        InventoryManager.Entity.add(this);

        // -- Disable until use().
        gameObject.SetActive(false);
    }



    // -- UsableItemInterface method
    public void use() {

        // -- If player has a flashlight and its not already full.
        gameObject.SetActive(true);
        flashlight.increaseEnergy(100.0f);

        InventoryManager.Entity.remove(this);
        Destroy(gameObject);

        // -- else do nothing.

    }
    public ItemData getItemData(){
        return referenceData;
    }
}
