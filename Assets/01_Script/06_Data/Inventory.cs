using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public class InventoryChangedEvent : GameAction<Inventory> { }

    InventoryChangedEvent inventoryChangedEvent;

    public List<Item> items; // 아이템
    public int capacity; // 총 공간
    ItemDataSO none; // 빈 공간 처리용

    public Inventory()
    {
        items = new List<Item>();
        inventoryChangedEvent = new InventoryChangedEvent();

        EventManager.Instance.AddEvent<InventoryChangedEvent>(inventoryChangedEvent);
        DataManager.Instance.TryGetItemData(AddressKeys.None, out none);
        FillInventory(null);
    }

    public void FillInventory(List<ItemId> list)
    {
        DataManager.Instance.TryGetItemData(AddressKeys.Sword, out ItemDataSO item1);
        DataManager.Instance.TryGetItemData(AddressKeys.Katana, out ItemDataSO item2);
        WeaponItemDataSO weapon = item2 as WeaponItemDataSO;

        items.Clear();
        items.Add(new WeaponItem(weapon, false));
        items.Add(new WeaponItem(weapon, false));
        items.Add(new(item1));
        items.Add(new(item1));
        for (int i = 0; i < 20; i++)
            items.Add(new(none));
    }

    /// <summary>
    /// 아이템을 인벤토리에 추가한다.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    { 
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].data.id == ItemId.None)
            {
                items[i] = item;
                items[i].PickUp();
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
        if (items[index].data.id == ItemId.None)
            return;

        items[index].Drop(); // stat을 빼거나 장비해제하거나

        // DropItem 생성, player 위치에 떨어뜨린다.
        if (AssetManager.Instance.TryGetAsset<GameObject>(AddressKeys.DropItem, out GameObject dropItem))
        {
            IInItable obj = GameObject.Instantiate(dropItem).GetComponent<DropItem>();
            obj.Initialize(new DropItemArg
            {
                position = GameManager.Instance.player.transform.position,
                item = items[index]
            });
        }
        else
        {
            Debug.Log($"Asset {AddressKeys.DropItem} is null");
        }

        // 떨궜으니 인벤을 비운다.
        items[index] = new Item(none);
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
        inventoryChangedEvent.Invoke(this);
    }

    public void SwapItem(int index1, int index2)
    {
        Item temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
        inventoryChangedEvent.Invoke(this);
    }

    public void UnEquipItem(WeaponItem weapon)
    {
        if (weapon == null)
            return;

        int index = items.IndexOf(weapon);

        UseItem(index);
    }
}
