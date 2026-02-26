using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RangedAttack : Skill
{
    RangedAttackSO rData;

    private float currentAngle;
    private float boxOffset;

    public RangedAttack(SkillDataSO data, SkillContext ctx) : base(data, ctx)
    {
        rData = data as RangedAttackSO;
        boxOffset = rData.attackRange / 2;
    }

    public override async UniTask Activate(Transform target, int value, CancellationToken ct)
    {
        if (isRunning) return;
        if (target == null) return;
        if (indicators.Count < 1) return;

        try
        {
            isRunning = true;
            // 방향 고정
            Vector2 dir = (target.position - owner.transform.position).normalized;
            currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // 인디케이터 연출
            indicators[0].gameObject.SetActive(true);
            indicators[0].position = (Vector2)owner.transform.position + (dir * (boxOffset - rData.attackRange / 2));
            indicators[0].rotation = Quaternion.Euler(0, 0, currentAngle);
            indicators[0].localScale = new Vector3(rData.attackRange, 0.2f, 1);
            indicatorRenderer[0].color = new Color(1, 0, 0, 0.2f); // 연한 빨강

            // 선 딜레이
            await UniTask.Delay(TimeSpan.FromSeconds(rData.startDelay), cancellationToken: ct);

            // 애니메이션 실행
            ShowAnim(rData.animTime, ct).Forget();
            indicators[0].gameObject.SetActive(false); // 인디케이터 비활성화

            await UniTask.Delay(TimeSpan.FromSeconds(rData.attackTime), cancellationToken: ct);

            // 실제 공격 판정
            GameObject obj = PoolManager.Instance.Instanciate(rData.id,
            new BulletArg { id = rData.id, atk = value, dir = dir, speed = rData.speed, pos = owner.transform.position });

            // 후 딜레이
            await UniTask.Delay(TimeSpan.FromSeconds(rData.endDelay), cancellationToken: ct);

            // 쿨타임 시작
            SetCooltime(rData.coolTime, ct).Forget();
        }
        catch (System.OperationCanceledException)
        {
            if (indicators != null && indicators[0] != null)
            {
                indicators[0].gameObject.SetActive(false);
            }

            if (anim != null)
            {
                anim?.SetBool("IsAttack", false);
            }
        }
        finally
        {
            isRunning = false;
        }
    }

    private async UniTaskVoid ShowAnim(float animTime, CancellationToken ct)
    {
        anim.SetBool("IsAttack", true); // 애니메이션 실행

        await UniTask.Delay(TimeSpan.FromSeconds(animTime), cancellationToken: ct).SuppressCancellationThrow();
        if (anim == null) return;

        anim.SetBool("IsAttack", false); // 애니메이션 종료
    }
}
