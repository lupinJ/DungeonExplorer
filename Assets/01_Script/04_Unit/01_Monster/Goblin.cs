using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Goblin : Monster
{
    [SerializeField] private float attackRange = 1.2f; // 공격 시도 범위
    [SerializeField] private float chaceRange = 4.0f; // 추적 범위

    [SerializeField] private float attackDelay = 0.8f; // 공격 딜레이
    [SerializeField] private float animStart = 0.7f; // 애니 시작
    [SerializeField] private float attackStart = 0.1f; // 공격 시작
    [SerializeField] private float animEnd = 0.1f; // 애니 종료

    [SerializeField] private Vector2 boxSize = new Vector2(1.5f, 1.0f); // 공격 범위
    [SerializeField] private float boxOffset = 0.75f; // box.x / 2 밀어줌

    [SerializeField] private Transform indicator;  
    [SerializeField] private SpriteRenderer indicatorRenderer; // 알파값 조절용

    private Transform target; // 공격 대상 (플레이어)
    private float currentAngle = 0f;
    private Vector2 currentBoxCenter = Vector2.zero;
    private CancellationTokenSource cts; // enable or die시 cancel();

    SelectorNode rootNode; // behavior tree

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="data"></param>
    public override void Initialize(InitData data = null)
    {
        base.Initialize(data);
        if (DataManager.Instance.TryGetMonsterData(AddressKeys.GoblinData, out MonsterDataSO monster))
        {
            target = GameManager.Instance.player.transform;
            stat.InitStat(monster.stat);
            movement.Speed = monster.stat.speed;
            stat.onDie += Die;

            BuildBT();

            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();

            FixedMoveAsync(cts.Token).Forget();
            RunBTRoutine(cts.Token).Forget();
        }
    }

    /// <summary>
    /// BT 생성
    /// </summary>
    private void BuildBT()
    {
        // 공격 시퀀스: 사거리 체크 -> 공격 실행
        var attackSequence = new SequenceNode();
        attackSequence.Add(new ActionNode(CheckAttackRange));
        attackSequence.Add(new ActionNode(DoAttackAction));

        // 추적 시퀀스: 추적 범위 체크 -> 추적 실행
        var chaseSequence = new SequenceNode();
        chaseSequence.Add(new ActionNode(CheckChaseRange));
        chaseSequence.Add(new ActionNode(DoChaseAction));

        // 루트 선택: 공격 > 추적 > 대기
        var selector = new SelectorNode();
        selector.Add(attackSequence);
        selector.Add(chaseSequence);
        selector.Add(new ActionNode(DoIdleAction));

        rootNode = selector;
    }

    /// <summary>
    /// BT 루프
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async UniTaskVoid RunBTRoutine(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await rootNode.Evaluate(ct);
            await UniTask.NextFrame(PlayerLoopTiming.Update, ct);
        }
    }

    #region BT Action Methods

    // [조건] 공격 사거리 확인
    private async UniTask<INode.State> CheckAttackRange(CancellationToken ct)
    {
        if (target == null) return INode.State.Failure;
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= attackRange ? INode.State.Success : INode.State.Failure;
    }

    // [행동] 실제 공격 프로세스
    private async UniTask<INode.State> DoAttackAction(CancellationToken ct)
    {
        movement.Dir = Vector2.zero; // 공격 시 정지
        // 이미 구현하신 AttackProcess를 await로 실행 (UniTaskVoid -> UniTask로 변경 권장)
        await AttackProcess(ct);
        return INode.State.Success;
    }

    // [조건] 추적 범위 확인
    private async UniTask<INode.State> CheckChaseRange(CancellationToken ct)
    {
        if (target == null) return INode.State.Failure;
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= chaceRange ? INode.State.Success : INode.State.Failure;
    }

    // [행동] 추적 실행
    private async UniTask<INode.State> DoChaseAction(CancellationToken ct)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        movement.Dir = dir; // 이동
        anim.SetBool("IsMove", true); // 필요 시 애니메이션 추가
        return INode.State.Running; // 계속 추적 중임을 알림
    }

    // [행동] 대기 
    private async UniTask<INode.State> DoIdleAction(CancellationToken ct)
    {
        movement.Dir = Vector2.zero;
        anim.SetBool("IsMove", false);
        return INode.State.Success;
    }

    #endregion

    /// <summary>
    /// 물리 이동 처리
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    async UniTaskVoid FixedMoveAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
            rigid.velocity = movement.Velocity;
        }
    }

    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        stat.onDie -= Die;
    }

    /// <summary>
    /// 고블린 공격
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask AttackProcess(CancellationToken token)
    {
        try
        {
            // 방향 고정
            Vector2 dir = (target.position - transform.position).normalized;
            currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currentBoxCenter = (Vector2)transform.position + (dir * boxOffset);

            // 인디케이터 연출
            indicator.gameObject.SetActive(true);
            indicator.position = (Vector2)transform.position + (dir * (boxOffset - boxSize.x / 2));
            indicator.rotation = Quaternion.Euler(0, 0, currentAngle);
            indicator.localScale = new Vector3(0, boxSize.y, 1);
            indicatorRenderer.color = new Color(1, 0, 0, 0.2f); // 연한 빨강

            // 가로 스케일을 boxSize.x까지 키우고, 색상을 진하게 만듭니다.
            indicator.DOScaleX(boxSize.x, attackDelay).SetEase(Ease.Linear)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken: token).Forget();
            indicatorRenderer.DOFade(0.6f, attackDelay)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken: token).Forget();

            await UniTask.Delay((int)(animStart * 1000), cancellationToken: token);

            anim.SetBool("IsAttack", true); // 애니메이션 실행

            await UniTask.Delay((int)(attackStart * 1000), cancellationToken: token);

            ExecuteBoxAttack(); // 실제 공격 판정
            indicator.gameObject.SetActive(false); // 인디케이터 비활성화

            await UniTask.Delay((int)(animEnd * 1000), cancellationToken: token);

            anim.SetBool("IsAttack", false); // 애니메이션 종료
            
        }
        catch (System.OperationCanceledException) 
        {
            //indicator.gameObject?.SetActive(false);
            anim.SetBool("IsAttack", false);
        }
    }

    /// <summary>
    /// 실제 공격 처리
    /// </summary>
    private void ExecuteBoxAttack()
    {
        LayerMask playerLayer = LayerMask.GetMask("Player");

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[5];
        int count = Physics2D.OverlapBox(currentBoxCenter, boxSize, currentAngle, filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i].isTrigger && results[i].TryGetComponent<IHitable>(out var hitable))
            {
                hitable.Hit(stat.Atk);
            }
        }
    }

    /// <summary>
    /// 고블린 사망
    /// </summary>
    /// <param name="die"></param>
    public override void Die(bool die)
    {
        if (isDead) return; 
        isDead = true;

        // 비동기 처리
        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        // 물리 처리
        rigid.velocity = Vector2.zero;
        rigid.isKinematic = true; 
        
        // 인디케이터 처리
        indicator.gameObject.SetActive(false);

        // 애니메이션 처리
        anim.SetBool("IsAttack", false); 
        anim.SetBool("IsDead", true);

        // 아이템 드랍, n초후 destroy() 필요
    }

#if UNITY_EDITOR
    // 에디터 창에서 공격 범위를 그리는 코드
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // 실행 중일 때 계산된 실시간 범위 표시
            Gizmos.color = Color.red;
            DrawRotatedBox(currentBoxCenter, boxSize, currentAngle);
        }
        else if (target != null)
        {
            Gizmos.color = Color.yellow;
            DrawRotatedBox(currentBoxCenter, boxSize, currentAngle);
        }
    }

    private void DrawRotatedBox(Vector2 center, Vector2 size, float angle)
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0.1f));
        Gizmos.matrix = Matrix4x4.identity; // 매트릭스 초기화
    }
#endif

    
}
