using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItemDataSO", menuName = "Scriptable Object/WeaponItemDataSO")]
public class WeaponItemDataSO : ItemDataSO
{
    [Header("=== Weapon Info ===")]
    public int atk;
    public float atk_range;
    public GameObject weapon;
}
