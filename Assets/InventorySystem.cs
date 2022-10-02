using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Entity;

    [SerializeField] private GameObject slotPrefab;
    private InventorySlot[] slots;

    private List< List<SelectableInterface> > inventory;
    private int displayedSlots;
    private int firstSlotIndex;

    private DescriptionOverlay overlay;



    void Awake() {
        Entity = this;
    }

    void Start() {
        displayedSlots = 5;
        firstSlotIndex = 0;

        overlay = GameObject.Find("Item Overlay").GetComponent<DescriptionOverlay>();

        inventory = new List< List<SelectableInterface> >();
        slots = new InventorySlot[displayedSlots];

        for (int i = 0; i < displayedSlots; i++) {
            // -- Instantiate as Children.
            slots[i] = Instantiate(slotPrefab, gameObject.transform).GetComponent<InventorySlot>();
        }

    }



    public void add(SelectableInterface item) {
        int index = searchForItemList(item.getItemData());

        if (index == -1) {
            // -- Display Item Overlay scene.
            overlay.DisplayOverlay(item.getItemData(), false);
            inventory.Add(new List<SelectableInterface>() { item }); 
        }
        else { inventory[index].Add(item); }

        updateInventoryUI();
    }

    public void remove(SelectableInterface item) {
        int index = searchForItemList(item.getItemData());

        if (index != -1) {
            if (inventory[index].Count > 1) { 
                inventory[index].RemoveAt(0); 
            }
            else { inventory.RemoveAt(index); }
            updateInventoryUI();
        }
    }


    public void itemLeftClick(int slot) {
        if (slot >= inventory.Count) { return; }
        if (inventory[slot].Count <= 0) { return; }

        // -- Use item.
        inventory[slot][0].use();
    }

    public void itemRightClick(int slot) {
        if (slot >= inventory.Count) { return; }
        if (inventory[slot].Count <= 0) { return; }

        // -- Display the Item description.
        overlay.DisplayOverlay(inventory[slot][0].getItemData(), false);
    }



    public void scrollInventory(int slotDx) {
        if (inventory.Count <= displayedSlots) { return; }
        firstSlotIndex = (firstSlotIndex + slotDx) % inventory.Count;

        updateInventoryUI();
    }


    private void updateInventoryUI() {
        int activeSlots = Mathf.Min(inventory.Count, displayedSlots);

        // -- Fill each active slot.
        for (int i = 0; i < activeSlots; i++) {
            int index = (firstSlotIndex + i) % inventory.Count;
            slots[i].set(inventory[index][0].getItemData(), inventory[index].Count);
        }

        // -- Set remaining slots to Empty.
        for (int i = activeSlots; i < displayedSlots; i++){
            slots[i].setEmpty();
        }

    }


    // -- O(n), but n is no more than 100.
    private int searchForItemList(ItemData item) {
        string name = item.itemName;
        for(int i = 0; i < inventory.Count; i++) {
            if (inventory[i][0].getItemData().itemName == name){
                return i;
            }
        }
        return -1; // -- No item found.
    }


}