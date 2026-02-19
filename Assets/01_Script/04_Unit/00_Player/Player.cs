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

    PlayerStat stat; // player 스텟
    Movement movement; // 움직임 계산
    Skill dash; // dash 스킬

    CancellationTokenSource enableToken; // 토큰
    Camera mainCamera; // 카메라 캐싱

    [SerializeField] SkillDataSO dashDataSO; // 대쉬 스킬 정보
    [SerializeField] GameObject weponParent; // 무기 회전 object
    [SerializeField] Weapon weapon; // 장착한 무기
    

    float interactRange; // 상호작용 범위
    bool isFlip = false; // 바라보는 방향
    Vector3 attackDir;

    protected override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;
        stat = new PlayerStat();
        inventory = new Inventory();
        movement = new Movement();
        dash = new DashSkill(dashDataSO, new SkillContext
        {
            owner = this,
            movement = this.movement,
            stat = this.stat,
            indicator = null
        });
        movement.Speed = 5f;
        interactRange = 1.0f;
        isFlip = false;
        stat.Atk = 10;
    }
    public void Initialize(InitData data = default)
    {
        EventManager.Instance.Subscribe<InputManager.MoveEvent, MoveArgs>(Move);
        EventManager.Instance.Subscribe<InputManager.DashEvent, InputState>(Dash);
        EventManager.Instance.Subscribe <InputManager.InteractEvent, InputState>(Interact);
        EventManager.Instance.Subscribe<InputManager.AttackEvent, InputState>(Attack);

        stat.onDie += PlayerDie;
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

        dash = null;
    }

    public bool TryHeal(int value)
    {
        if(stat.Hp == stat.MaxHp)
            return false;

        stat.Hp += value;
        return true;
    }

    public bool ManaRecovery(int value)
    {
        if (stat.Mp == stat.MaxMp)
            return false;

        stat.Mp += value;
        return true;
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
    /// 보는 방향 처리
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    async UniTaskVoid LookAtAsync(CancellationToken ct)
    {
        while(!ct.IsCancellationRequested)
        {
            // 마우스 방향 수집
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            bool nextFilp = mousePos.x < transform.position.x;

            // flip 처리
            if (isFlip != nextFilp)
            {
                Flip(nextFilp);           
            }

            // 무기 회전 처리
            if(weapon != null && !weapon.IsAttack)
            {
                Vector2 direction = (mousePos - weponParent.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Vector3 weaponScale = weponParent.transform.localScale;
                weaponScale.y = (mousePos.x < transform.position.x) ? -1f : 1f;
                weponParent.transform.localScale = weaponScale;

                weponParent.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            await UniTask.NextFrame(ct);
        }
    }

    void Flip(bool flip)
    {
        isFlip = flip;
        sprite.flipX = flip; 
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

        dash.Activate(null, 0, enableToken.Token);
        //DashAsync(enableToken.Token).Forget();
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
    /// <summary>
    /// 범위 시각화 도구
    /// </summary>
    private void OnDrawGizmos()
    {
        if (weponParent == null || stat == null) return;

        Vector2 origin = weponParent.transform.position;
        // 마우스 방향 계산
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 forward = ((Vector2)mousePos - origin).normalized;

        // 원형 범위 그리기
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawSphere(origin, stat.Range);

        // +- 60도 선 그리기
        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, 60) * forward * stat.Range;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -60) * forward * stat.Range;

        Gizmos.DrawLine(origin, (Vector3)origin + (Vector3)forward * stat.Range); // 중앙선
        Gizmos.DrawLine(origin, (Vector3)origin + leftBoundary);
        Gizmos.DrawLine(origin, (Vector3)origin + rightBoundary);
    }
#endif
    /// <summary>
    /// player 무기 장착
    /// </summary>
    /// <param name="obj"></param>
    public void Equip(WeaponItem item)
    {
        if (item == null)
            return;

        if (weapon != null)
            inventory.UnEquipItem(weapon.Item);

        // 부모 초기화
        weponParent.transform.rotation = Quaternion.identity;
        weponParent.transform.localScale = Vector3.one;

        // 무기 생성
        weapon = Instantiate(item.Prefab).GetComponent<Weapon>();
        weapon.transform.SetParent(weponParent.transform);

        weapon.transform.localPosition = new Vector2(0.6f, -0.3f);
        weapon.transform.localRotation = Quaternion.identity;

        weapon.Initialize(new WeaponArg { item = item });

        stat.Atk += item.wdata.atk;
        stat.Range += item.wdata.atk_range;
    }

    /// <summary>
    /// 장착 해제
    /// </summary>
    /// <param name="item"></param>
    public void UnEquip(WeaponItem item)
    {
        if (item == null || weapon == null)
            return;

        if (weapon.Item != item)
            return;

        stat.Atk -= item.wdata.atk;
        stat.Range -= item.wdata.atk_range;

        // 무기 해제
        Destroy(weapon.gameObject);
        weapon = null;
    }

    /// <summary>
    /// 피격시 처리
    /// </summary>
    /// <param name="atk"></param>
    public void Hit(int atk)
    {
        if (stat.IsInvincible)
            return;

        stat.Hp -= atk;
        stat.InvincibleAsync(0.2f, enableToken.Token).Forget(); // 피격 무적
    }

    /// <summary>
    /// 공격 처리
    /// </summary>
    /// <param name="state"></param>
    public void Attack(InputState state)
    {
        if (state != InputState.Started)
            return;

        attackDir = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        weapon?.Attack();

    }

    /// <summary>
    /// 공격 범위 탐색
    /// </summary>
    public void OnHitCheck()
    {
        Vector2 attackOrigin = weponParent.transform.position;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        // 필터 설정
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;      
        filter.useLayerMask = true;

        Collider2D[] targets = new Collider2D[15];
        int count = Physics2D.OverlapCircle(attackOrigin, stat.Range, filter, targets);
        Vector2 forward = ((Vector2)attackDir - attackOrigin).normalized; ;

        for (int i = 0; i < count; i++)
        {
            if (targets[i] == null) continue;
            
            Vector2 dirToTarget = ((Vector2)targets[i].transform.position - attackOrigin).normalized;
            float dot = Vector2.Dot(forward, dirToTarget);

            // Dot 0.5는 정면 기준 +-60도(총 120도) 범위
            if (dot >= 0.5f)
            {
                if (targets[i] == null || targets[i].isTrigger == false) continue;

                if (targets[i].TryGetComponent<IHitable>(out var hitable))
                {
                    hitable.Hit(stat.Atk); 
                }
            }
        }
    }

    public void PlayerDie(bool isDie)
    {
        // 사망 애니메이션
        // 움직임 봉쇄
        // GameOver처리(GameManager) - Event 연결 or 직접 호출
    }
}
