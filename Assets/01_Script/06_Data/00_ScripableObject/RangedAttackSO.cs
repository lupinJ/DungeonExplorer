using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttackDataSO", menuName = "Scriptable Object/RangedAttackDataSO")]
public class RangedAttackSO : SkillDataSO
{
    [Header("=== Ranged Attack Bullet ===")]
    public BulletId id;
    public float speed;
}
