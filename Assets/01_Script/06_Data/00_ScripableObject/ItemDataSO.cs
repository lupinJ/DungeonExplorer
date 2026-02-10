using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemId
{
    None,
    Sword,
    Katana
}

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Object/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public ItemId id;
    public string itemName;
    public string tooltip;

    public int maxCount;
    public int price;

    public Sprite image;
    
}
