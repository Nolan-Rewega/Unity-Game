using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableManager : MonoBehaviour
{
    public static EquipableManager Entity;

    private EquipableItemInterface m_currentEquipedItem;
    private bool m_forceDisarm;

    // Start is called before the first frame update
    void Start(){
        m_currentEquipedItem = null;
        m_forceDisarm = false;

        Entity = this;
    }


    public EquipableItemInterface getEquipedItem() {
        return m_currentEquipedItem;
    }


    public void setEquipedItem(EquipableItemInterface _item) {
        EquipableItemInterface item = (m_forceDisarm) ? null : _item;

        // -- Equip the new Item.
        if (item != null){
            item.equip();
        }

        // -- Unequip previous item.
        if (m_currentEquipedItem != null){
            m_currentEquipedItem.unequip();
        }

        // -- Change the current item
        m_currentEquipedItem = item;
    }


    // -- Only called by ForceDisarm script.
    public void setForceDisarm(bool forceDisarm) {
        m_forceDisarm = forceDisarm;
    }


    public bool getIsPlayerLightSourceOn() {
        // -- might not work
        if (m_currentEquipedItem == null) { return false; }

        var lightsrc = m_currentEquipedItem.GetType().GetInterface("PlayerLightSource");
        return (lightsrc != null) ? ((PlayerLightSource)m_currentEquipedItem).getIsLightSourceOn() : false;
    }


}
