using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected Animator anim;
    protected SpriteRenderer sprite;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }
}

public interface IHitable
{
    public void Hit(int atk);
}
