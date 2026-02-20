using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    public class HpEvent : GameAction<PointArg> { }
    public class MpEvent : GameAction<PointArg> { }

    HpEvent hpEvent;
    MpEvent mpEvent;

    public PlayerStat() : base()
    {
        hpEvent = new HpEvent();
        mpEvent = new MpEvent();

        EventManager.Instance.AddEvent(hpEvent);
        EventManager.Instance.AddEvent(mpEvent);

        this.onHpChanged += OnHpChanged;
        this.onMpChanged += OnMpChanged;

        this.Range = 0f;
        this.Atk = 0;
    }

    public void Reset()
    {
        EventManager.Instance.RemoveEvent<HpEvent>();
        EventManager.Instance.RemoveEvent<MpEvent>();
    }

    public void OnHpChanged(PointArg arg)
    {
        hpEvent?.Invoke(arg);
    }

    public void OnMpChanged(PointArg arg)
    {
        mpEvent?.Invoke(arg);
    }
}
