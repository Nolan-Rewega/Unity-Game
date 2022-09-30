using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private GameObject inventory;
    private bool isOpen;

    void Start() {
        inventory = GameObject.Find("Inventory");
        inventory.SetActive(false);
        isOpen = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            isOpen = !isOpen;
            inventory.SetActive(isOpen);
            
        }

        int scrolldelta = (int)Input.mouseScrollDelta.y;
        if (isOpen && scrolldelta != 0) {
            InventorySystem.Entity.scrollInventory(scrolldelta);
        }

    }
}
