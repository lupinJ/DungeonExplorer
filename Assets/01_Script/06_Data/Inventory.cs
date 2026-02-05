using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public class InventoryChangedEvent : GameAction<Inventory> { }

    InventoryChangedEvent inventoryChangedEvent;

    public List<Item> items;
    public int capacity;
    ItemDataSO none;

    public Inventory()
    {
        items = new List<Item>();
        inventoryChangedEvent = new InventoryChangedEvent();
        EventManager.Instance.AddEvent<InventoryChangedEvent>(inventoryChangedEvent);
        DataManager.Instance.TryGetItemData("Assets/06_Data/ScriptableObject/ItemDataSO/None.asset", out none);
        FillInventory(null);
    }

    public void FillInventory(List<ItemId> list)
    {
        bool ch = DataManager.Instance.TryGetItemData("Assets/06_Data/ScriptableObject/ItemDataSO/None.asset", out ItemDataSO item1);
        ch = DataManager.Instance.TryGetItemData("Assets/06_Data/ScriptableObject/ItemDataSO/Sword.asset", out ItemDataSO item2);
        if (!ch)
            return;
        items.Add(new(item2));
        items.Add(new(item2));
        items.Add(new(item2));
        for (int i = 0; i < 20; i++)
            items.Add(new(item1));
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가한다.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        

        inventoryChangedEvent.Invoke(this);
    }

    public void DropItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return;

        items[index] = new Item(none);
        inventoryChangedEvent.Invoke(this);
    }

    public void UseItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return;

        items[index].Use();
        inventoryChangedEvent.Invoke(this);
    }

    public void SwapItem(int index1, int index2)
    {
        Item temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
        inventoryChangedEvent.Invoke(this);
    }
}
