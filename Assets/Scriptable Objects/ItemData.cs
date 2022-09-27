using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data Object")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
}
