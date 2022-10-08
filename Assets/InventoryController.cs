using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private DescriptionOverlay overlay;
    private GameObject inventory;
    private PlayerMovement player;
    private bool isinventoryOpen;

    void Awake() {
        inventory = GameObject.Find("Inventory");
        overlay   = GameObject.Find("Item Overlay").GetComponent<DescriptionOverlay>();
        player    = GameObject.Find("Player").GetComponent<PlayerMovement>();

        inventory.SetActive(false);
        isinventoryOpen = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab) && !overlay.getIsOverlayOpen()) {
            isinventoryOpen = !isinventoryOpen;

            // -- Stop movement if inventory is open.
            player.haltPlayerCamera(isinventoryOpen);
            inventory.SetActive(isinventoryOpen);
        }

        int scrolldelta = (int)Input.mouseScrollDelta.y;
        if (isinventoryOpen && scrolldelta != 0) {
            InventoryManager.Entity.scrollInventory(scrolldelta);
        }

    }
}
