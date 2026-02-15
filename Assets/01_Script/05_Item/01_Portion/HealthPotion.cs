using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : PotionItem
{
    
    public override void Use()
    {
        bool is_sucess = GameManager.Instance.player.TryHeal(pData.value);

        if (is_sucess)
            Count--;
    }
}
