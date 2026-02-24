using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemId
{
    None,
    HealthPotion,
    ManaPotion,
    Sword,
    Katana
}

[System.Serializable]
public struct ItemDropInfo
{
    public ItemId id;
    public int count;

    [Range(0f, 1f)]
    public float dropRate;

    ItemDropInfo(ItemId id, int count = 1, float dropRate = 1f)
    {
        this.id = id;
        this.count = count;
        this.dropRate = dropRate;
    }
}
public static class ItemFactory
{
    private static readonly Dictionary<ItemId, Func<Item>> ItemCreators = new()
    {
        { ItemId.None, () => new Item() },
        { ItemId.Sword, () => new Item() },
        { ItemId.Katana, () => new WeaponItem() },
        { ItemId.HealthPotion, () => new HealthPotion() },
        { ItemId.ManaPotion, () => new ManaPotion() },
    };

    public static Item CreateItem(ItemId id)
    {
        if (ItemCreators.TryGetValue(id, out var creator))
        {
            Item item = creator.Invoke();

            if (DataManager.Instance.TryGetItemData(id, out var data))
            {
                item.Initialize(new ItemArg { itemDataSO = data, count = 1 });
            }

            return item;
        }

        Debug.LogError($"아이템 ID {id}에 해당하는 생성 로직이 없습니다.");
        return null;
    }

    public static Item CreateItem(ItemDropInfo info)
    {
        if (ItemCreators.TryGetValue(info.id, out var creator))
        {
            Item item = creator.Invoke();

            // 데이터(SO) 연결 로직 (DataManager 등 활용)
            if (DataManager.Instance.TryGetItemData(info.id, out var data))
            {
                item.Initialize(new ItemArg { itemDataSO = data, count = info.count });
            }

            return item;
        }

        Debug.LogError($"아이템 ID {info.id}에 해당하는 생성 로직이 없습니다.");
        return null;
    }
}
