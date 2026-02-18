using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Goblin : Monster
{
    [SerializeField] private Transform indicator;  
    
    SelectorNode rootNode; // behavior tree
    Skill skill; // monster Attack

    protected override void Awake()
    {
        base.Awake();
        skill = new MeleeAttack(skillData, new SkillContext { owner = this, indicator = indicator });
        BuildBT();
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="data"></param>
    public override void Initialize(InitData data = null)
    {
        base.Initialize(data);
  
        skill.Reset();

        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        Birth(cts.Token).Forget();
    }

    private async UniTaskVoid Birth(CancellationToken ct)
    {
        anim.SetBool("IsBirth", true);
        stat.IsInvincible = true;

        await UniTask.Delay(TimeSpan.FromSeconds(data.birthTime), cancellationToken: ct);

        anim.SetBool("IsBirth", false);
        stat.IsInvincible = false;

        FixedMoveAsync(ct).Forget();
        RunBTRoutine(ct).Forget();
    }

    /// <summary>
    /// BT 생성
    /// </summary>
    private void BuildBT()
    {
        // 공격 시퀀스: 사거리 체크 -> 쿨차임 체크 -> 공격 실행
        var attackSequence = new SequenceNode();
        attackSequence.Add(new ActionNode(CheckAttackRange));
        attackSequence.Add(new ActionNode(CheckAttackCoolTime));
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
#pragma warning disable CS1998
    // [조건] 공격 사거리 확인
    private async UniTask<INode.State> CheckAttackRange(CancellationToken ct)
    {
        if (target == null) return INode.State.Failure;
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= skill.AttackRange ? INode.State.Success : INode.State.Failure;
    }

    private async UniTask<INode.State> CheckAttackCoolTime(CancellationToken ct)
    {
        if (target == null) return INode.State.Failure;
        return skill.CoolTime == 0 ? INode.State.Success : INode.State.Failure;
    }

    // [행동] 실제 공격 프로세스
    private async UniTask<INode.State> DoAttackAction(CancellationToken ct)
    {
        movement.Dir = Vector2.zero; // 공격 시 정지
        anim.SetBool("IsMove", false);

        await skill.Activate(target, stat.Atk, ct);
        return INode.State.Success;
    }

    // [조건] 추적 범위 확인
    private async UniTask<INode.State> CheckChaseRange(CancellationToken ct)
    {
        if (target == null) return INode.State.Failure;
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= data.ChaseRange.y && dist > data.ChaseRange.x ? INode.State.Success : INode.State.Failure;
    }

    // [행동] 추적 실행
    private async UniTask<INode.State> DoChaseAction(CancellationToken ct)
    {
        Vector2 dir = (target.position - transform.position).normalized;

        if(dir.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        movement.Dir = dir; // 이동
        anim.SetBool("IsMove", true); 
        return INode.State.Running; // 계속 추적 중임을 알림
    }

    // [행동] 대기 
    private async UniTask<INode.State> DoIdleAction(CancellationToken ct)
    {
        movement.Dir = Vector2.zero;
        anim.SetBool("IsMove", false);
        return INode.State.Success;
    }
#pragma warning restore CS1998
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

    private void OnDestroy()
    {
        skill = null;
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
        DieAsync(2.0f).Forget();
    }

    public async UniTaskVoid DieAsync(float time)
    {
        await UniTask.WaitForSeconds(time);
        PoolManager.Instance.Destroy(this.gameObject);
    }

#if UNITY_EDITOR
    // 에디터 창에서 공격 범위를 그리는 코드
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // 실행 중일 때 계산된 실시간 범위 표시
            Gizmos.color = Color.red;
            //DrawRotatedBox(currentBoxCenter, boxSize, currentAngle);
        }
        else if (target != null)
        {
            Gizmos.color = Color.yellow;
            //DrawRotatedBox(currentBoxCenter, boxSize, currentAngle);
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
