using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MelleAttackDataSO", menuName = "Scriptable Object/MelleAttackDataSO")]
public class MelleeAttackSO : SkillDataSO
{
    [Header("=== Mellee Attack Range ===")]

    public Vector2 BoxRange; // 공격 범위
}
