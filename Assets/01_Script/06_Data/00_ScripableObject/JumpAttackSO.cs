using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAttackDataSO", menuName = "Scriptable Object/JumpAttackDataSO")]
public class JumpAttackSO : SkillDataSO
{
    [Header("=== Round Attack Info ===")]
    public float attackRadius; // 원형 공격범위 반지름
    public float upAnimTime; // up 애니메이션 시간
    public float upDelay; // 공중에 머무르는 시간
}
