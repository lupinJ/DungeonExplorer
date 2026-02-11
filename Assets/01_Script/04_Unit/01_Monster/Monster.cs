using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MonsterArg : InitData
{
    public Vector2 position;
}

public abstract class Monster : Unit, IHitable, IInItable
{
    protected Stat stat;
    protected Movement movement;
    protected bool isDead; 
    protected override void Awake()
    {
        base.Awake();
        stat = new Stat();
        movement = new Movement();
    }
    public virtual void Hit(int atk)
    {
        stat.Hp -= atk;
        Debug.Log($"Hit : hp = {stat.Hp}");
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
