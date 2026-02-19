using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : PotionItem
{
    public override void Use()
    {
        if (Count <= 0) return;

        bool is_sucess = GameManager.Instance.player.ManaRecovery(pData.value);

        if (is_sucess)
            Count--;
    }
}
