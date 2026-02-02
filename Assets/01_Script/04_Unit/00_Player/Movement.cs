using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private Vector2 dir;
    private Vector2 attractForce;
    private float speed;

    int isStop;
    int isPause;

    bool IsStop
    {
        get
        {
            if (isStop == 0)
                return false;
            else
                return true;
        }
        set
        {
            if (value == true)
                isStop += 1;
            else
                isStop -= 1;
        }
    }
    public Movement()
    {
        this.dir = Vector2.zero;
        attractForce = Vector2.zero;
        this.speed = 0;
        isStop = 0;
        isPause = 0;
    }

    public Vector2 Velocity
    {
        get
        {
            // ÀÏ½ÃÁ¤Áö
            if (isPause != 0) 
                return Vector2.zero;

            // ¿òÁ÷ÀÓ ºÀ¼â
            if (isStop != 0)
                return attractForce;
            
            return (dir * speed) + attractForce;
        }
    }

    public Vector2 Dir
    {
        set
        {
            dir = value.normalized;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }
    

}
