using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum BulletId
{
    None,
    Arrow,
}

public struct BulletArg : InitData
{
    public BulletId id;
    public int atk;
    public float speed;
    public Vector2 dir;
    public Vector2 pos;
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, IPoolable, IInItable
{
    Rigidbody2D rigid;
    CancellationTokenSource cts;

    BulletId id;
    Vector2 dir;

    int wallLayer;
    int playerLayer;

    int atk;
    float speed;
    bool isDeactivate;

    readonly float maxDuration = 15f; // 넘으면 destroy 

    private void Awake()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        wallLayer = LayerMask.NameToLayer("Wall");
        playerLayer = LayerMask.NameToLayer("Player");
        isDeactivate = false;
    }
    public void Initialize(InitData data = null)
    {
        if (data is not BulletArg arg)
            return;

        atk = arg.atk;
        speed = arg.speed;
        id = arg.id;
        dir = arg.dir.normalized;
        transform.position = arg.pos;
        isDeactivate = false;

        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        Fire();
    }

    public void Fire()
    {
        // 투사체 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 발사
        rigid.velocity = dir * speed;

        // 타이머 시작
        DestroyTimer(maxDuration, cts.Token);
    }

    private async void DestroyTimer(float time, CancellationToken ct)
    {
        bool cancel = await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct)
            .SuppressCancellationThrow();
        if (cancel) return;

        Deactivate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 벽 충돌
        if (collision.gameObject.layer == wallLayer)
        {
            Deactivate();
        }

        // Player 타격
        if (collision.gameObject.layer == playerLayer)
        {
            collision.GetComponent<IHitable>().Hit(atk);
            Deactivate();
        }

    }

    private void Deactivate()
    {
        if (isDeactivate) return;

        isDeactivate = true;
        PoolManager.Instance.Destroy(this.gameObject);
    }

    private void OnDisable()
    {
        isDeactivate = false;

        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }
    public void OnSpawn()
    {
        rigid.velocity = Vector3.zero;
        atk = 0;
        speed = 0f;

        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }
    public void OnDespawn() { }
}

