using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Scriptable Object/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    [Header("=== Item Basic Info ===")]
    public ItemId id;
    public string itemName;
    public string tooltip;
    public int maxCount;
    public int price;

    public Sprite image;
    
}
