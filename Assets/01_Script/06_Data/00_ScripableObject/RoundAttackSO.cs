using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundAttackDataSO", menuName = "Scriptable Object/RoundAttackDataSO")]
public class RoundAttackSO : SkillDataSO
{
    [Header("=== Round Attack Info ===")]
    public BulletId id;
    public int count;
    public float speed;
}
