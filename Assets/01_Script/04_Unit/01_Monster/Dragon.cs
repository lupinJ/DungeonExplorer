using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Dragon : Monster
{
    [SerializeField] private Transform indicator;

    [Header("Dragon Extra Skill")]
    [SerializeField] private SkillDataSO jumpSkillData;
    [SerializeField] private SkillDataSO RoundSkillData;

    Skill jumpSkill;
    Skill roundSkill;

    protected override void Awake()
    {
        base.Awake();
        skill = new RangedAttack(skillData, new SkillContext { owner = this, indicator = new List<Transform> { indicator } });
        BuildBT();
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="data"></param>
    public override void Initialize(InitData data = null)
    {
        base.Initialize(data);
        Birth(cts.Token).Forget();
    }

    protected override async UniTaskVoid Birth(CancellationToken ct)
    {
        anim.SetBool("IsDown", true);
        stat.InvincibleAsync(data.birthTime, ct).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(data.birthTime), cancellationToken: ct);

        anim.SetBool("IsDown", false);

        SoundManager.Instance.PlayBgm(SoundId.BossBgm);
        FixedMoveAsync(ct).Forget();
        RunBTRoutine(ct).Forget();
    }

    /// <summary>
    /// BT 생성
    /// </summary>
    protected override void BuildBT()
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
        Vector2 dir = (target.position - transform.position).normalized;

        if (dir.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

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

        if (dir.x < 0)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        movement.Dir = dir; // 이동
        return INode.State.Running; // 계속 추적 중임을 알림
    }

    // [행동] 대기 
    private async UniTask<INode.State> DoIdleAction(CancellationToken ct)
    {
        movement.Dir = Vector2.zero;
        return INode.State.Success;
    }
#pragma warning restore CS1998
    #endregion
    public override void Die(bool die)
    {
        if (isDead) return;
        isDead = true;

        // 정지처리
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        // 물리 처리
        rigid.velocity = Vector2.zero;

        // 인디케이터 처리
        indicator.gameObject.SetActive(false);

        // 애니메이션 처리
        anim.SetBool("IsUp", false);
        anim.SetBool("IsDown", false);
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsDead", true);

        // 배경음악 처리
        SoundManager.Instance.PlayMainBgm();

        // 아이템 드랍, n초후 destroy() 필요
        DieAsync(2.0f, cts.Token).Forget();
    }

}
