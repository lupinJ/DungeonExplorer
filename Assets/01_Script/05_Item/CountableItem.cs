using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CountableItemArg : InitData
{
    public ItemDataSO itemDataSO;
    public int count;
}

public class CountableItem : Item
{
    int count;

    public int Count
    {
        get { return count; }
        set { count = value; }
    }
    public CountableItem()  { }

    public override void Initialize(InitData data)
    {
        if(data is ItemArg ItemArg)
        {
            this.data = ItemArg.itemDataSO as ItemDataSO;
            count = 1;
        }
        else if (data is CountableItemArg CountArg)
        {
            this.data = CountArg.itemDataSO as ItemDataSO;
            count = CountArg.count;
        }

    }
  
}
