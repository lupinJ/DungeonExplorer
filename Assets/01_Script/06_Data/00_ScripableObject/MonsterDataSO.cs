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
    public MonsterId id;
    public string Monstername;
    public StatData stat;
    // 드랍 테이블
}
