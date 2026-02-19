using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : PotionItem
{
    
    public override void Use()
    {
        if (Count <= 0) return;

        bool is_sucess = GameManager.Instance.player.TryHeal(pData.value);

        if (is_sucess)
            Count--;
    }
}
