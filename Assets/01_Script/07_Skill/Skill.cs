using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct SkillContext
{
    public Unit owner;
    public Movement movement;
    public Stat stat;
    public Transform indicator;
}

public abstract class Skill 
{
    protected SkillDataSO data;

    protected Unit owner; // 공격 주체
    protected Animator anim; // 주체 애니메이션
    protected Movement movement; // 주체 움직임
    protected Stat stat; // 주체 스텟

    protected Transform indicator; // 공격 표시
    protected SpriteRenderer indicatorRenderer; // 색 or 투명도 조절

    protected float curCooltime; // 현재 쿨타임
    protected bool isRunning; // 스킬 진행중 flag

    public float CoolTime => curCooltime;
    public float AttackRange => data.attackRange;
    public int MpCost => data.mpCost;

    public Skill(SkillDataSO data, SkillContext ctx)
    {
        this.data = data;
        owner = ctx.owner;
        anim = owner.GetComponent<Animator>();
        movement = ctx.movement;
        stat = ctx.stat;
        indicator = ctx.indicator;
        indicatorRenderer = indicator?.GetComponent<SpriteRenderer>();
        curCooltime = 0;
    }

    public abstract UniTask Activate(Transform target, int value, CancellationToken ct);
    public virtual void Reset() { curCooltime = 0; }

    protected async UniTaskVoid SetCooltime(float cooltime, CancellationToken ct)
    {
        if (data.coolTime == 0)
            return;

        curCooltime = data.coolTime;

        while (!ct.IsCancellationRequested)
        {
            curCooltime -= Time.deltaTime;
            if (curCooltime <= 0)
            {
                curCooltime = 0;
                break;
            }
            await UniTask.NextFrame(ct);
        }
    }
}



