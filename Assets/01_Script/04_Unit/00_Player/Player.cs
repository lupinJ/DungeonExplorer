using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Unit
{
    Movement movement;
    CancellationTokenSource enableToken;

    protected override void Awake()
    {
        base.Awake();

        movement = new Movement();
        movement.Speed = 5f;
    }

    private void OnEnable()
    {
        enableToken = new CancellationTokenSource();
        FixedMoveAsync(enableToken.Token).Forget();
        LookAtAsync(enableToken.Token).Forget();
    }

    private void OnDisable()
    {
        enableToken.Cancel();
        enableToken.Dispose();
    }

    async UniTaskVoid Start()
    {
        await UniTask.NextFrame(this.destroyCancellationToken);

        EventManager.Instance.Subscribe<InputManager.MoveEvent, MoveArgs>(Move);
        EventManager.Instance.Subscribe<InputManager.DashEvent, InputState>(Dash);
    }

    private void OnDestroy()
    {
        if(EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<InputManager.MoveEvent, MoveArgs>(Move);
        }        
    }

    /// <summary>
    /// 물리 이동 연산 함수
    /// </summary>
    /// <returns></returns>
    async UniTaskVoid FixedMoveAsync(CancellationToken ct)
    {
        while(true)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
            rigid.velocity = movement.Velocity;
        }
    }

    /// <summary>
    /// 마우스 방향으로 flip
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    async UniTaskVoid LookAtAsync(CancellationToken ct)
    {
        while(true)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x < transform.position.x)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }
            await UniTask.NextFrame(ct);
        }
    }

    /// <summary>
    /// player 움직임 처리
    /// </summary>
    /// <param name="args"></param>
    private void Move(MoveArgs args)
    {
        if (args.state == InputState.Started)
            anim.SetBool("IsMove", true);
        else if (args.state == InputState.Canceled)
            anim.SetBool("IsMove", false);
        movement.Dir = args.dir;
    }

    public void Dash(InputState state)
    {
        DashAsync().Forget();
    }

    /// <summary>
    /// player 대쉬 처리
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid DashAsync()
    {
        
        anim.SetBool("IsDash", true);
        movement.Speed += 5f;

        await UniTask.WaitForSeconds(0.2f, false, PlayerLoopTiming.Update, this.destroyCancellationToken);

        movement.Speed -= 5f;
        anim.SetBool("IsDash", false);
    }
}
