using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CountableItem : Item
{
    int count;

    public int Count
    {
        get { return count; }
        set { count = value; }
    }

    public int MaxCount => data.maxCount;
    
    public CountableItem()  { }

    public override void Initialize(InitData data)
    {
        if(data is ItemArg ItemArg)
        {
            this.data = ItemArg.itemDataSO as ItemDataSO;
            count = ItemArg.count;
        }

    }
  
}
