using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct ItemArg : InitData
{
    public ItemDataSO itemDataSO;
}

public class Item : IInItable
{
    protected ItemDataSO data;

    public ItemId Id => data.id;
    public Sprite Image => data.image;

    public Item() { }

    public virtual void Initialize(InitData data) 
    {
        if(data is ItemArg arg)
        {
            this.data = arg.itemDataSO;
        }
    }

    public virtual void Use()
    {
        if (data.id == ItemId.None)
            return;
    }

    public virtual void PickUp() { }
    public virtual void Drop() { }

}
