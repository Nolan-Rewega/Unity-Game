using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableManager : MonoBehaviour
{
    public static EquipableManager Entity;

    EquipableItemInterface currentEquipedItem;

    // Start is called before the first frame update
    void Start(){
        currentEquipedItem = null;

        Entity = this;
    }

    public EquipableItemInterface getEquipedItem() {
        return currentEquipedItem;
    }

    public void setEquipedItem(EquipableItemInterface item) {

        // -- Equip the new Item.
        if (item != null){
            item.equip();
        }

        // -- Unequip previous light source.
        if (currentEquipedItem != null){
            currentEquipedItem.unequip();
        }

        // -- Change the current light source
        currentEquipedItem = item;
    }

    public bool getIsPlayerLightSourceOn() {
        // -- might not work
        if (currentEquipedItem == null) { return false; }

        var lightsrc = currentEquipedItem.GetType().GetInterface("PlayerLightSource");
        return (lightsrc != null) ? ((PlayerLightSource)currentEquipedItem).getIsLightSourceOn() : false;
    }


}
