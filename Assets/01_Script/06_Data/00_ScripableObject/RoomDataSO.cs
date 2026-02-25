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
    public List<MonsterSpawnInfo> spawnData = new();

    [Header("=== Treasure Box Info ===")]
    public List<TreasureBoxArg> boxData = new();

    [Header("=== Portal Info ===")]
    public List<PortalArg> portalData = new();
}
