using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// -- Pair data structure
class Pair<T1, T2> {
    public Pair(T1 item1, T2 item2){
        this.Item1 = item1;
        this.Item2 = item2;
    }
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
}


public class InventorySystem : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    private List<Pair<ItemData, int>> inventory;
    private int displayedSlots;
    private int firstSlotIndex;

    private GameObject[] slots;


    void Start() {
        displayedSlots = 5;
        firstSlotIndex = 0;

        inventory = new List<Pair<ItemData, int>>();
        slots = new GameObject[displayedSlots];

        for (int i = 0; i < displayedSlots; i++) {
            slots[i] = Instantiate(slotPrefab, gameObject.transform);
        }

    }



    public void add(ItemData item) {
        int index = searchForItem(item);

        if (index == -1) {
            inventory.Add(new Pair<ItemData, int>(item, 1));
        }
        else {
            inventory[index].Item2++;
        }
        updateInventoryUI();
    }

    public void remove(ItemData item) {
        int index = searchForItem(item);

        if (index != -1) {
            if (inventory[index].Item2 > 1) {
                inventory[index].Item2--;
            }
            else {
                inventory.RemoveAt(index);
            }
            updateInventoryUI();
        }

    }

    public void shiftWindow(int slotDx) {
        // -- Prevent shifting on insufficent items.
        if (inventory.Count <= 6) { return; }
        firstSlotIndex = (firstSlotIndex + slotDx) % inventory.Count;

        updateInventoryUI();
    }


    public void updateInventoryUI() {
        int activeSlots = Mathf.Min(inventory.Count, displayedSlots);

        // -- Fill each active slot.
        for (int i = 0; i < activeSlots; i++) {
            int index = (firstSlotIndex + i) % inventory.Count;
            slots[i].GetComponent<InventorySlot>().set(inventory[index].Item1, inventory[index].Item2);
        }

        // -- Set remaining slots to Empty.
        for (int i = activeSlots; i < displayedSlots; i++){
            slots[i].GetComponent<InventorySlot>().setEmpty();
        }

    }


    // -- O(n), but n is no more than 100.
    private int searchForItem(ItemData item) {
        string ID = item.itemID;
        for(int i = 0; i < inventory.Count; i++) {
            if (inventory[i].Item1.itemID == ID){
                //Debug.Log("FOUND ITEM: " + item.itemName + ", At position: " + i);
                return i;
            }
        }
        //Debug.Log("DIDNT FIND ITEM: " + item.itemName);
        return -1; // -- No item found.
    }



}
