using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matches : MonoBehaviour, SelectableInterface
{
    [SerializeField] private ItemData referenceData;

    // Start is called before the first frame update
    void Start(){
        
    }

    public void onPickUp() {
        // -- Adds Matches to the players inventory
        InventorySystem.Entity.add(this);

        // -- Disable until use().
        gameObject.SetActive(false);
    }

    // -- When Clicked or Q pressed.
    public void use(){
        // -- Use the Item and remove it from the game.
        gameObject.SetActive(true);

        InventorySystem.Entity.remove(this);
        Destroy(gameObject);
    }


    public ItemData getItemData(){
        return referenceData;
    }

}
