using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Bullet
{
    public override void Fire()
    {
        // 투사체 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // 발사
        rigid.velocity = dir * speed;

        // 타이머 시작
        DestroyTimer(maxDuration, cts.Token);
    }
}
