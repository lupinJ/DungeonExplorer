using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    public Goblin() : base() {}

    public override void Initialize(InitData data = null)
    {
        base.Initialize(data);
        if (DataManager.Instance.TryGetMonsterData(AddressKeys.GoblinData, out MonsterDataSO monster))
        {
            stat.InitStat(monster.stat);
            movement.Speed = monster.stat.speed;
        }
    }
}
