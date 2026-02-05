using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InventoryUI : PopupUI, IInItable
{
    [SerializeField] List<SlotUI> slots;
    Inventory inventory;
     
    public override PopupUI GetPanel()
    {
        return this;
    }

    public void Initialize(InitData data = default)
    {
        EventManager.Instance.Subscribe<Inventory.InventoryChangedEvent, Inventory>(OnInventoryChanged);
        this.inventory = GameManager.Instance.player.inventory;
        
        for(int i=0; i <  slots.Count; i++)
        {
            slots[i].index = i;
            slots[i].onItemSwap += SlotSwap;
            slots[i].onItemClicked += SlotClick;
            slots[i].onItemDrop += SlotDrop;
        }

        OnInventoryChanged(this.inventory);
        gameObject.SetActive(false);
    }

    public void SlotSwap(int s1, int s2) 
    {
        inventory.SwapItem(s1, s2);
    }
    public void SlotClick(int index)
    {
        inventory.UseItem(index);
    }
    public void SlotDrop(int index)
    {
        inventory.DropItem(index);
    }

    void OnInventoryChanged(Inventory inventory) 
    {
        List<Item> list = inventory.items;
        for(int i = 0; i < list.Count; i++)
        {
            slots[i].item = list[i];
            slots[i].RefreshUI();
        }
        
        for(int i = list.Count; i < slots.Count; i++)
        {
            slots[i].item = null;
            slots[i].RefreshUI();
        }
    }
}
