using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDataSO", menuName = "Scriptable Object/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    [Header("=== Skill Basic Info ===")]
    public string skillName; // skill 이름

    public float coolTime; // 쿨타임
    public int mpCost; // Mp 소모량
    public float attackRange; // 공격 사거리

    public float startDelay; // 선딜
    public float animTime; // 공격 모션 시간
    public float endDelay; // 후딜    
}

