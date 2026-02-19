using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : Npc, IPoolable
{
    public override void Interact()
    {
        anim.SetBool("IsOpen", true);
    }

    public void OnDespawn()
    {
        
    }

    public void OnSpawn()
    {
        rigid.velocity = Vector3.zero;
        anim.SetBool("IsOpen", false);
    }
}
