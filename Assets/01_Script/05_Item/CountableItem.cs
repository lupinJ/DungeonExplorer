using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItem : Item
{
    int count;

    public int Count
    {
        get { return count; }
        private set { count = value; }
    }
    public CountableItem(ItemDataSO data, int count) : base(data)
    {
        this.count = count;
    }

    
  
}
