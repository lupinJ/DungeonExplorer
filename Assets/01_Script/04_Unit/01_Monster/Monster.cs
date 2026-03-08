using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct MonsterArg : InitData
{
    public Vector2 position;
}

[System.Serializable]
public struct MonsterSpawnInfo
{
    public MonsterId id;
    public Vector2 localPosition;
}


public abstract class Monster : Unit,
    IHitable, IInItable, IPoolable
{
    [Header("Monster Info")]
    [SerializeField] protected MonsterDataSO data; // 몬스터 정보
    [SerializeField] protected SkillDataSO skillData; // 공격 정보
    [SerializeField] protected SimpleHpBarUI hpBar; // 몬스터 hpBar UI

    protected Stat stat;
    protected Movement movement;
    protected SelectorNode rootNode; // behavior tree
    protected Skill skill; // monster Attack
    protected bool isDead;

    protected Transform target; // 공격 대상 (플레이어)
    protected CancellationTokenSource cts; // enable or die시 cancel();

    public event Action<Monster> OnDie;
    
    protected override void Awake()
    {
        base.Awake();
        stat = new Stat();
        movement = new Movement();
        isDead = false;
        target = null;
        cts = null;
    }
    protected abstract void BuildBT();
    protected abstract UniTaskVoid Birth(CancellationToken ct);
    public abstract void Die(bool die);

    /// <summary>
    /// BT 루프
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected async UniTaskVoid RunBTRoutine(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await rootNode.Evaluate(ct);
            await UniTask.NextFrame(PlayerLoopTiming.Update, ct);
        }
    }

    /// <summary>
    /// 물리 이동 처리
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected async UniTaskVoid FixedMoveAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
            rigid.velocity = movement.Velocity;
        }
    }
    protected async UniTaskVoid DieAsync(float time, CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct);
        PoolManager.Instance.Destroy(this.gameObject);
    }

    public virtual void Hit(int atk)
    {
        if (stat.IsInvincible)
            return;
        
        stat.Hp -= atk;
    }

    public virtual void Initialize(InitData data = null)
    {
        if(data is not MonsterArg monsterArg)
            return;

        transform.position = monsterArg.position;
        target = GameManager.Instance.player.transform;
        stat.InitStat(this.data.stat);
        skill.Reset();
        movement.Speed = this.data.stat.speed;
        hpBar.OnHpChanged(new PointArg { current = stat.MaxHp, max = stat.MaxHp });

        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
    }

    private void OnMonsterDie(bool die)
    {
        OnDie?.Invoke(this);
        Die(true);
    }

    public void SetHpBarActive(bool active)
    {
        if (hpBar == null)
            return;
        if (hpBar.gameObject == null)
            return;
        hpBar.gameObject.SetActive(active);
    }

    public void OnSpawn()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rigid.velocity = Vector3.zero;
        rigid.isKinematic = false;
        isDead = false;

        stat.onHpChanged -= hpBar.OnHpChanged;
        stat.onHpChanged += hpBar.OnHpChanged;
        stat.onDie -= OnMonsterDie;
        stat.onDie += OnMonsterDie;
        
    }
    public void OnDespawn()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        stat.onDie -= OnMonsterDie;
        stat.onHpChanged -= hpBar.OnHpChanged;
    }

    private void OnDisable()
    {
        OnDespawn();
    }
    protected virtual void OnDestroy()
    {
        skill = null;
    }
}
