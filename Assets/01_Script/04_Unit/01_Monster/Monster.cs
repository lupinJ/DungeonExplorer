using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

struct MonsterArg : InitData
{
    public Vector2 position;
}

public abstract class Monster : Unit,
    IHitable, IInItable, IPoolable
{
    [SerializeField] protected MonsterDataSO data; // 몬스터 정보
    [SerializeField] protected SkillDataSO skillData; // 공격 정보
    [SerializeField] protected SimpleHpBarUI hpBar; // 몬스터 hpBar UI

    protected Stat stat;
    protected Movement movement;
    protected bool isDead;

    protected Transform target; // 공격 대상 (플레이어)
    protected CancellationTokenSource cts; // enable or die시 cancel();
    
    protected override void Awake()
    {
        base.Awake();
        stat = new Stat();
        movement = new Movement();
        isDead = false;
        target = null;
        cts = null;
    }

    public virtual void Hit(int atk)
    {
        if (stat.IsInvincible)
            return;
        
        stat.Hp -= atk;
    }

    public virtual void Initialize(InitData data = null)
    {
        if(data is MonsterArg monsterArg)
        {
            transform.position = monsterArg.position;

            target = GameManager.Instance.player.transform;
            stat.InitStat(this.data.stat);
            movement.Speed = this.data.stat.speed;
            hpBar.OnHpChanged(new PointArg { current = stat.MaxHp, max = stat.MaxHp });
        }
    }



    public abstract void Die(bool die);

    public void OnSpawn()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rigid.velocity = Vector3.zero;
        rigid.isKinematic = false;
        isDead = false;

        stat.onHpChanged -= hpBar.OnHpChanged;
        stat.onHpChanged += hpBar.OnHpChanged;
        stat.onDie -= Die;
        stat.onDie += Die;
        
    }

    public void OnDespawn()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        stat.onDie -= Die;
        stat.onHpChanged -= hpBar.OnHpChanged;
    }

    private void OnDisable()
    {
        OnDespawn();
    }
}
