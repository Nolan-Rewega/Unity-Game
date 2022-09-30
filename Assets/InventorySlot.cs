using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private Image itemIcon;
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemAmount;

    private int slotNumber;

    void Awake(){
        slotNumber = gameObject.transform.parent.childCount - 1;
        Transform[] children = gameObject.transform.GetComponentsInChildren<Transform>();
        
        // -- Locate Slot Components.
        foreach (Transform t in children){
            if (t.gameObject.name == "Item Name"){
                itemName = t.gameObject.GetComponent<TextMeshProUGUI>();
            }
            if (t.gameObject.name == "Item Amount"){
                itemAmount = t.gameObject.GetComponent<TextMeshProUGUI>();
            }
            if (t.gameObject.name == "Item Icon"){
                itemIcon = t.gameObject.GetComponent<Image>();
            }
        }

        setEmpty();
    }
    
    public void set(ItemData item, int amount) {
        itemIcon.enabled = true;
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;
        itemAmount.text = amount.ToString();
    }
    public void setEmpty() {
        itemIcon.enabled = false;
        itemName.text = "";
        itemAmount.text = "";
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left){
            InventorySystem.Entity.itemLeftClick(slotNumber);
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            InventorySystem.Entity.itemRightClick(slotNumber);
        }
    }

}
