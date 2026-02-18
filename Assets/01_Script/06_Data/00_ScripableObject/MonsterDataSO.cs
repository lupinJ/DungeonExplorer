using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterId
{
    None,
    Goblin,
}

[CreateAssetMenu(fileName = "MonsterDataSO", menuName = "Scriptable Object/MonsterDataSO")]
public class MonsterDataSO : ScriptableObject
{
    [Header("=== Monster Basic Info ===")]
    public MonsterId id; // 고유 id
    public string monsterName; // 몬스터 이름
    public float birthTime; // 스폰 시간
    public Vector2 ChaseRange; // 추적범위 (x ~ y까지)
    public StatData stat;
    // 드랍 테이블
}
