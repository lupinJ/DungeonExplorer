using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Unit, IInItable, IHitable
{
    public Inventory inventory;

    Stat stat;
    Movement movement;
    CancellationTokenSource enableToken;

    protected override void Awake()
    {
        base.Awake();

        stat = new Stat();
        inventory = new Inventory();
        movement = new Movement();
        movement.Speed = 5f;
    }
    public void Initialize(InitData data = default)
    {
        EventManager.Instance.Subscribe<InputManager.MoveEvent, MoveArgs>(Move);
        EventManager.Instance.Subscribe<InputManager.DashEvent, InputState>(Dash);
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

    private void OnDestroy()
    {
        if(EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<InputManager.MoveEvent, MoveArgs>(Move);
            EventManager.Instance.Unsubscribe<InputManager.DashEvent, InputState>(Dash);
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
        if (state != InputState.Started)
            return;

        if (stat.Mp < 10)
            return;
        stat.Mp -= 10;

        DashAsync().Forget();
    }

    /// <summary>
    /// player 대쉬 처리
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid DashAsync()
    {
        
        anim.SetBool("IsDash", true);
        movement.Speed += 12f;

        await UniTask.WaitForSeconds(0.2f, false, PlayerLoopTiming.Update, this.destroyCancellationToken);

        movement.Speed -= 12f;
        anim.SetBool("IsDash", false);
    }

    public void Hit(int atk)
    {
        stat.Hp -= atk;
    }
}
