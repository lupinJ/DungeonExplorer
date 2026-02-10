using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MonsterArg : InitData
{
    public Vector2 position;
}

public class Monster : Unit, IHitable, IInItable
{
    protected Stat stat;
    protected Movement movement;

    protected override void Awake()
    {
        base.Awake();
        stat = new Stat();
        movement = new Movement();
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
        }
    }
}
