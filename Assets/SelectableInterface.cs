using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SelectableInterface
{
    public ItemData getItemData();
    public void onPickUp();
    public void use();
}
