using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : CountableItem
{
    protected PotionDataSO pData;

    public override void Initialize(InitData data)
    {
        base.Initialize(data);
        if (data is CountableItemArg arg)
        {
            pData = arg.itemDataSO as PotionDataSO;
        }
    }
}
