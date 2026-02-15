using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : PotionItem
{
    public override void Use()
    {
        bool is_sucess = GameManager.Instance.player.ManaRecovery(pData.value);

        if (is_sucess)
            Count--;
    }
}
