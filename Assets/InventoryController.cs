using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private DescriptionOverlay overlay;
    private GameObject inventory;
    private bool isinventoryOpen;

    void Awake() {
        inventory = GameObject.Find("Inventory");
        overlay = GameObject.Find("Item Overlay").GetComponent<DescriptionOverlay>();

        inventory.SetActive(false);
        isinventoryOpen = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab) && !overlay.getIsOverlayOpen()) {
            isinventoryOpen = !isinventoryOpen;
            inventory.SetActive(isinventoryOpen);
        }

        int scrolldelta = (int)Input.mouseScrollDelta.y;
        if (isinventoryOpen && scrolldelta != 0) {
            InventorySystem.Entity.scrollInventory(scrolldelta);
        }

    }
}
