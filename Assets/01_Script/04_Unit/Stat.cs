using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PointArg
{
    public int current, max;
}

[System.Serializable]
public struct StatData
{
    public int maxHp;
    public int maxMp;
    public int atk;
    public float atk_range;
    public float speed;
}

public class Stat 
{
    public event Action<PointArg> onHpChanged;
    public event Action<PointArg> onMpChanged;
    public event Action<bool> onDie;

    int maxHp;
    int hp;

    int maxMp;
    int mp;

    int atk;
    float atkRange;

    public Stat()
    {
        // юс╫ц ╫╨ех
        maxHp = 100;
        hp = 100;
        maxMp = 100;
        mp = 100;

        atk = 10;
        atkRange = 2f;
    }

    public Stat(StatData data)
    {
        InitStat(data);
    }

    public void InitStat(StatData data)
    {
        maxHp = data.maxHp;
        hp = maxHp;
        maxMp = data.maxMp;
        mp = maxMp;
        atk = data.atk;
        atkRange = data.atk_range;
    }


    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }
    public int MaxMp
    {
        get { return maxMp; }
        set { maxMp = value; }
    }
    public int Hp
    {
        get { return hp; }
        set
        {
            if (value < 0)
                hp = 0;
            else if (value > maxHp)
                hp = maxHp;
            else
            {
                hp = value;
            }

            onHpChanged?.Invoke(new PointArg { current = hp, max = maxHp });
            if (hp <= 0)
                onDie?.Invoke(true);

        }
    }

    public int Mp
    {
        get { return mp; }
        set
        {
            if (value < 0)
                mp = 0;
            else if (value > maxMp)
                mp = maxMp;
            else
            {
                mp = value;
            }
            onMpChanged?.Invoke(new PointArg { current = mp, max = maxMp });
            
        }
    }

    public int Atk
    {
        get { return atk; }
        set
        {
            atk = value;
        }
    }

    public float Range
    {
        get { return atkRange; }
        set { atkRange = value; }
    }

}
