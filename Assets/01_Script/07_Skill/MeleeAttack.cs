using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeleeAttack : Skill
{
    MelleeAttackSO mData;

    private float currentAngle;
    private Vector2 currentBoxCenter;
    private float boxOffset;

    public MeleeAttack(SkillDataSO data, SkillContext ctx) : base(data, ctx)
    {
        mData = data as MelleeAttackSO;

        currentAngle = 0f;
        currentBoxCenter = Vector2.zero;
        boxOffset = mData.BoxRange.x / 2;
    }

    public override async UniTask Activate(Transform target, int atk, CancellationToken ct)
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
            indicator.position = (Vector2)owner.transform.position + (dir * (boxOffset - mData.BoxRange.x / 2));
            indicator.rotation = Quaternion.Euler(0, 0, currentAngle);
            indicator.localScale = new Vector3(0, mData.BoxRange.y, 1);
            indicatorRenderer.color = new Color(1, 0, 0, 0.2f); // 연한 빨강

            // 가로 스케일을 boxSize.x까지 키우고, 색상을 진하게 만듭니다.
            indicator.DOScaleX(mData.BoxRange.x, mData.startDelay).SetEase(Ease.Linear)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken: ct).Forget();
            indicatorRenderer.DOFade(0.6f, mData.startDelay)
                .ToUniTask(TweenCancelBehaviour.Kill, cancellationToken: ct).Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(mData.startDelay), cancellationToken: ct);

            anim.SetBool("IsAttack", true); // 애니메이션 실행
            indicator.gameObject.SetActive(false); // 인디케이터 비활성화

            await UniTask.Delay(TimeSpan.FromSeconds(mData.animTime), cancellationToken: ct);

            ExecuteBoxAttack(atk); // 실제 공격 판정
            anim.SetBool("IsAttack", false); // 애니메이션 종료

            await UniTask.Delay(TimeSpan.FromSeconds(mData.endDelay), cancellationToken: ct);

            SetCooltime(mData.coolTime, ct).Forget(); // 쿨타임 시작
        }
        catch (System.OperationCanceledException)
        {
            if (indicator != null)
            {
                indicator.DOKill();
                indicator.gameObject.SetActive(false);
            }

            if (anim != null)
            {
                anim?.SetBool("IsAttack", false);
            }
        }
    }

    private void ExecuteBoxAttack(int atk)
    {
        LayerMask playerLayer = LayerMask.GetMask("Player");

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[5];
        int count = Physics2D.OverlapBox(currentBoxCenter, mData.BoxRange, currentAngle, filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i].isTrigger && results[i].TryGetComponent<IHitable>(out var hitable))
            {
                hitable.Hit(atk);
            }
        }
    }


}
