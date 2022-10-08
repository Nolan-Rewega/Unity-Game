using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matches : MonoBehaviour, SelectableInterface, UsableItemInterface
{
    [SerializeField] private ItemData referenceData;

    // Start is called before the first frame update
    void Start(){
        
    }

    public void onSelection(Vector3 playerPos) {
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 2.0f) { return; }

        // -- Adds Matches to the players inventory
        InventoryManager.Entity.add(this);

        // -- Disable until use().
        gameObject.SetActive(false);
    }

    // -- When Clicked or Q pressed.
    public void use(){
        // -- Use the Item and remove it from the game.
        gameObject.SetActive(true);

        InventoryManager.Entity.remove(this);
        Destroy(gameObject);
    }


    public ItemData getItemData(){
        return referenceData;
    }

}
