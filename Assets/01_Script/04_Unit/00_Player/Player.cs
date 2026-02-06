using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

interface IInteractable {
    public void Interact();
}

public class Player : Unit, IInItable, IHitable
{
    public Inventory inventory;

    Stat stat; // player 스텟
    Movement movement; // 움직임 계산
    CancellationTokenSource enableToken; // 토큰

    public float interactRange = 1.0f; // 상호작용 범위

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
        EventManager.Instance.Subscribe <InputManager.InteractEvent, InputState>(Interact);
    }

    private void OnEnable()
    {
        enableToken?.Cancel();
        enableToken?.Dispose();
        enableToken = new CancellationTokenSource();
        FixedMoveAsync(enableToken.Token).Forget();
        LookAtAsync(enableToken.Token).Forget();
    }

    private void OnDisable()
    {
        enableToken?.Cancel();
        enableToken?.Dispose();
        enableToken = null;
    }

    private void OnDestroy()
    {
        if(EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<InputManager.MoveEvent, MoveArgs>(Move);
            EventManager.Instance.Unsubscribe<InputManager.DashEvent, InputState>(Dash);
            EventManager.Instance.Unsubscribe<InputManager.InteractEvent, InputState>(Interact);
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

    
    public void Interact(InputState state)
    {
        if (state != InputState.Started)
            return;
        
        LayerMask npcLayer = LayerMask.GetMask("Npc"); 

        // Npc layer 서치
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, interactRange, npcLayer);

        if (targets.Length == 0) return;

        Collider2D closestTarget = null;
        float minDistance = float.MaxValue;

        // 거리 계산
        foreach (var target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = target;
            }
        }

        // 상호작용 실행
        if (closestTarget != null)
        {
            
            if (closestTarget.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }
            else
                Debug.Log($"try failed");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 원의 색상 설정
        Gizmos.color = Color.cyan;

        // 현재 오브젝트 위치에 반지름만큼의 원을 그림
        // OverlapCircle과 동일한 위치, 반지름 값을 넣어줍니다.
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
#endif
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
