using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    public WeaponItemDataSO wdata;
    bool is_equip;

    public GameObject Prefab => wdata.weapon;
    public bool Is_equip => is_equip;
    public WeaponItem() { }
    
    public override void Initialize(InitData data)
    {
        base.Initialize(data);

        if(data is ItemArg arg)
        {
            this.wdata = arg.itemDataSO as WeaponItemDataSO;
            is_equip = false;
        }
    }

    public override void Use()
    {
        // 장착 or 장착해제
        if (is_equip)
            UnEquip();
        else
            Equip();
    }

    /// <summary>
    /// 무기 장착
    /// </summary>
    public virtual void Equip()
    {
        is_equip = true;
        GameManager.Instance.player.Equip(this);
        
    }

    /// <summary>
    ///  무기 해제
    /// </summary>
    public virtual void UnEquip()
    {   
        is_equip = false;
        GameManager.Instance.player.UnEquip(this);
    }

    public override void Drop()
    {
        // 장착 되어있다면 해제
        if(is_equip)
            GameManager.Instance.player.UnEquip(this);
    }

}
