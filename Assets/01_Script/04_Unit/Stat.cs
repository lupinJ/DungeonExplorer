using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public struct PointArg
{
    public int current, max;
}

public class Stat 
{
    public class HpEvent : GameAction<PointArg> { }
    public class MpEvent : GameAction<PointArg> { }

    HpEvent hpEvent;
    MpEvent mpEvent;

    int maxHp;
    int hp;

    int maxMp;
    int mp;

    int atk;

    public Stat()
    {
        hpEvent = new HpEvent();
        mpEvent = new MpEvent();
        
        EventManager.Instance.AddEvent(hpEvent);
        EventManager.Instance.AddEvent(mpEvent);

        // юс╫ц ╫╨ех
        maxHp = 100;
        hp = 100;
        maxMp = 100;
        mp = 100;
        
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
                
            hpEvent.Invoke(new PointArg { current = hp, max = maxHp });
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

            mpEvent.Invoke(new PointArg { current = mp, max = maxMp });
        }
    }

}
