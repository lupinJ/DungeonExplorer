using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PotionDataSO", menuName = "Scriptable Object/PotionDataSO")]
public class PotionDataSO : ItemDataSO
{
    [Header("=== Potion Value ===")]
    public int value;
}
