using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : PopupUI, IInItable
{
    [SerializeField] SlotItemDragUI dragUI;
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
            slots[i].Index = i;
            slots[i].onItemSwap += SlotSwap;
            slots[i].onItemClicked += SlotClick;
            slots[i].onItemDrop += SlotDrop;
            slots[i].onDragStart += DragStart;
            slots[i].onDragEnd += DragEnd;
        }

        OnInventoryChanged(this.inventory);
        this.HidePanel();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<Inventory.InventoryChangedEvent, Inventory>(OnInventoryChanged);
        }
    }
    public void SlotSwap(int index1, int index2) 
    {
        inventory.SwapItem(index1, index2);
    }
    public void SlotClick(int index)
    {
        inventory.UseItem(index);
    }
    public void SlotDrop(int index)
    {
        inventory.DropItem(index);
    }

    public void DragStart(Item item)
    {
        dragUI.DragStart(item);
    }

    public void DragEnd()
    {
        dragUI.DragEnd();
    }

    void OnInventoryChanged(Inventory inventory) 
    {
        List<Item> list = inventory.items;
        for(int i = 0; i < list.Count; i++)
        {
            slots[i].Item = list[i];
            slots[i].RefreshUI();
        }
        
        for(int i = list.Count; i < slots.Count; i++)
        {
            slots[i].Item = null;
            slots[i].RefreshUI();
        }
    }
}
