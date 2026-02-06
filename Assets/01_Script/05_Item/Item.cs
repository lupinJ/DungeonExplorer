using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item 
{
    public ItemDataSO data;
    
    public Item(ItemDataSO data)
    {
        this.data = data;
    }

    public virtual void Use()
    {
        if (data.id == 0)
            return;
        Debug.Log($"Item {data.itemName} is used");
    }

    public virtual void PickUp() { }
    public virtual void Drop() { }

}
