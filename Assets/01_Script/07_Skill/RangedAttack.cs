using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RangedAttack : Skill
{
    RangedAttackSO rData;

    private float currentAngle;
    private Vector2 currentBoxCenter;
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

        try
        {
            // 방향 고정
            Vector2 dir = (target.position - owner.transform.position).normalized;
            currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currentBoxCenter = (Vector2)owner.transform.position + (dir * boxOffset);

            // 인디케이터 연출
            indicator.gameObject.SetActive(true);
            indicator.position = (Vector2)owner.transform.position + (dir * (boxOffset - rData.attackRange / 2));
            indicator.rotation = Quaternion.Euler(0, 0, currentAngle);
            indicator.localScale = new Vector3(rData.attackRange, 0.2f, 1);
            indicatorRenderer.color = new Color(1, 0, 0, 0.2f); // 연한 빨강

            await UniTask.Delay(TimeSpan.FromSeconds(rData.startDelay), cancellationToken: ct);

            anim.SetBool("IsAttack", true); // 애니메이션 실행
            indicator.gameObject.SetActive(false); // 인디케이터 비활성화

            await UniTask.Delay(TimeSpan.FromSeconds(rData.animTime), cancellationToken: ct);

            // 실제 공격 판정
            GameObject obj = PoolManager.Instance.Instanciate(BulletId.Arrow,
            new BulletArg { id = rData.id, atk = value, dir = dir, speed = rData.speed, pos = owner.transform.position });
            

            anim.SetBool("IsAttack", false); // 애니메이션 종료

            await UniTask.Delay(TimeSpan.FromSeconds(rData.endDelay), cancellationToken: ct);

            SetCooltime(rData.coolTime, ct).Forget(); // 쿨타임 시작
        }
        catch (System.OperationCanceledException)
        {
            if (indicator != null)
            {
                indicator.gameObject.SetActive(false);
            }

            if (anim != null)
            {
                anim?.SetBool("IsAttack", false);
            }
        }
    }
}
