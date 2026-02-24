using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomId
{
    StartRoom,
    CrossRoom,
    LeftRoom,
    RightRoom,
    BossRoom

}

[CreateAssetMenu(fileName = "RoomDataSO", menuName = "Scriptable Object/RoomDataSO")]
public class RoomDataSO : ScriptableObject
{
    [Header("=== Room Basic Info ===")]
    public RoomId id;
    public bool isEncount;

    [Header("=== Spawn Monster Info ===")]
    public MonsterSpawnInfo[] spawnData;

    [Header("=== Treasure Box Info ===")]
    public TreasureBoxArg[] boxData;
}
