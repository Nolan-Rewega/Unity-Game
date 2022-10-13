using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matches : MonoBehaviour, SelectableInterface, UsableItemInterface
{
    [SerializeField] private ItemData referenceData;
    private Lantern lantern;


    // Start is called before the first frame update
    void Start(){
        lantern = GameObject.Find("Lantern").GetComponent<Lantern>();
    }

    public void consumeMatch() {
        InventoryManager.Entity.remove(this);
        Destroy(this);
    }

    public void onSelection(Vector3 playerPos) {
        float distance = Vector3.Distance(playerPos, gameObject.transform.position);
        if (distance > 2.0f) { return; }

        // -- Adds Matches to the players inventory
        InventoryManager.Entity.add(this);

        // -- Disable until use().
        gameObject.SetActive(false);
    }

    // -- When Clicked or Q pressed.
    public void use(){ }


    public ItemData getItemData(){
        return referenceData;
    }

}
