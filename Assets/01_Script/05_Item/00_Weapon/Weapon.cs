using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct WeaponArg : InitData
{
    public WeaponItem item;
    public GameObject parent;
}

public abstract class Weapon : MonoBehaviour, IInItable
{
    SpriteRenderer sprite; // ¹«±â sprite
    WeaponItem item;
    protected bool isAttack;

    public WeaponItem Item
    {
        get
        {
            return item;
        }
        protected set { item = value; }
    }

    public bool IsAttack => isAttack;

    public abstract void Attack();

    public abstract void Initialize(InitData data = null);
   
    public virtual void Flip(bool flip) 
    {
        sprite.flipX = flip;
        float x = Mathf.Abs(transform.localPosition.x);
        transform.localPosition = flip ? new Vector2(-x, transform.localPosition.y) : new Vector2(x, transform.localPosition.y);
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

}
