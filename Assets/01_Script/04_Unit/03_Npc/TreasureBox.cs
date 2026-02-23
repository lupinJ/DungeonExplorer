using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct TreasureBoxArg : InitData
{
    public Vector2 position;
    public ItemDropInfo itemInfo;
}

public class TreasureBox : Npc, IPoolable, IInItable
{
    [SerializeField] ItemDropInfo itemInfo;
    bool isOpen;

    public void Initialize(InitData data = null)
    {
        if (data is not TreasureBoxArg arg)
            return;

        transform.localPosition = arg.position;
        itemInfo = arg.itemInfo;
    }

    public override void Interact(Player player)
    {
        if (isOpen || itemInfo.id == ItemId.None)
            return;

        player.inventory.AddItem(ItemFactory.CreateItem(itemInfo));
        itemInfo.id = ItemId.None;
        itemInfo.count = 0;

        anim.SetBool("IsOpen", true);
        isOpen = true;
    }

    public void OnDespawn()
    {
        
    }

    public void OnSpawn()
    {
        transform.position = Vector3.zero;
        rigid.velocity = Vector3.zero;
        anim.SetBool("IsOpen", false);
        isOpen = false;
    }
}
