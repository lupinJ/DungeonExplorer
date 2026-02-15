using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MonsterArg : InitData
{
    public Vector2 position;
}

public abstract class Monster : Unit, IHitable, IInItable
{
    protected MonsterDataSO data; // 몬스터 정보
    protected Stat stat;
    protected Movement movement;
    protected bool isDead; 

    protected override void Awake()
    {
        base.Awake();
        stat = new Stat();
        movement = new Movement();
        isDead = false;
    }
    public virtual void Hit(int atk)
    {
        stat.Hp -= atk;
    }

    public virtual void Initialize(InitData data = null)
    {
        if(data is MonsterArg monsterArg)
        {
            transform.position = monsterArg.position;
            rigid.velocity = Vector2.zero;
            rigid.isKinematic = false; 
        }
    }

    public abstract void Die(bool die);
}
