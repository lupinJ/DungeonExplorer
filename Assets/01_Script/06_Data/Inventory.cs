using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public class InventoryChangedEvent : GameAction<Inventory> { }

    InventoryChangedEvent inventoryChangedEvent;

    public List<Item> items; // 아이템
    readonly int maxInventory = 24;

    public Inventory()
    {
        items = new List<Item>();
        inventoryChangedEvent = new InventoryChangedEvent();

        EventManager.Instance.AddEvent<InventoryChangedEvent>(inventoryChangedEvent);
        FillInventory();
    }

    public void Reset()
    {
        EventManager.Instance.RemoveEvent<InventoryChangedEvent>();
    }
    public void FillInventory()
    {
        items.Clear();

        for (int i = 0; i < maxInventory; i++)
            items.Add(ItemFactory.CreateItem(ItemId.None));
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가한다.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        if (item == null || item.Id == ItemId.None)
            return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Id == ItemId.None)
            {
                items[i] = item;
                items[i].PickUp();
                break;
            }

            if (TryMoveCountableItem(item, items[i]))
            {
                if (item is not CountableItem cItem)
                    return;

                if (cItem.Count > 0)
                    continue;
                else
                    break;
            }
        }

        inventoryChangedEvent.Invoke(this);
    }

    /// <summary>
    /// 아이템을 인벤토리에서 빼고 바닥에 놓는다.
    /// </summary>
    /// <param name="index"></param>
    public void DropItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return;
        if (items[index].Id == ItemId.None)
            return;

        items[index].Drop(); // stat을 빼거나 장비해제하거나

        // DropItem 생성, player 위치에 떨어뜨린다.
        IInItable obj = PoolManager.Instance.Instanciate(AddressKeys.DropItem).GetComponent<DropItem>();
        obj.Initialize(new DropItemArg
        {
            position = GameManager.Instance.player.transform.position,
            item = items[index]
        });   

        // 떨궜으니 인벤을 비운다.
        items[index] = ItemFactory.CreateItem(ItemId.None);
        inventoryChangedEvent.Invoke(this);
    }

    /// <summary>
    /// 아이템을 사용한다.
    /// </summary>
    /// <param name="index"></param>
    public void UseItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return;

        items[index].Use();
        if (items[index] is CountableItem item && item.Count == 0)
            items[index] = ItemFactory.CreateItem(ItemId.None);

        inventoryChangedEvent.Invoke(this);
    }

    public void SwapItem(int index1, int index2)
    {
        if (index1 == index2)
            return;

        // 같은 물건을 합칠 경우
        if (TryMoveCountableItem(items[index1], items[index2]))
        {
            if (items[index1] is CountableItem item && item.Count == 0)
            {
                items[index1] = ItemFactory.CreateItem(ItemId.None);
            }
        }
        else
        {
            Item temp = items[index1];
            items[index1] = items[index2];
            items[index2] = temp;
        }

        inventoryChangedEvent.Invoke(this);
        
    }

    public void UnEquipItem(WeaponItem weapon)
    {
        if (weapon == null)
            return;

        int index = items.IndexOf(weapon);

        UseItem(index);
    }

    private bool TryMoveCountableItem(Item from ,  Item to)
    {
        if (from == null || to == null)
            return false;
        if (from.Id != to.Id)
            return false;
        if (from is not CountableItem cntFrom || to is not CountableItem cntTo)
            return false;

        int addCnt = Mathf.Min(cntTo.MaxCount - cntTo.Count, cntFrom.Count);
        cntTo.Count += addCnt;
        cntFrom.Count -= addCnt;

        return addCnt > 0;
    }
}
