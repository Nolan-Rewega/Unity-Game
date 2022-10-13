using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, SelectableInterface, UsableItemInterface, EquipableItemInterface
{
    [SerializeField] private ItemData referenceData;

    // -- EquipableItemInterface
    public void equip() {
        // -- Player animation
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public void unequip() {
        // -- Player animation
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }


    public void use() {
        EquipableManager.Entity.setEquipedItem(this);
    }

    // -- UsableItemInterface
    public ItemData getItemData(){
        return referenceData;
    }

    // -- SelectableInterface
    public void onSelection(Vector3 playerPos) {
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 1.0f) { return; }

        // -- Add to inventory
        InventoryManager.Entity.add(this);
        gameObject.SetActive(false);
    }


}
