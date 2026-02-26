using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class RoundAttack : Skill
{
    private RoundAttackSO rData; // 기존 데이터 구조 재활용 또는 전용 SO 사용

    public RoundAttack(SkillDataSO data, SkillContext ctx) : base(data, ctx)
    {
        rData = data as RoundAttackSO;
    }

    public override async UniTask Activate(Transform target, int value, CancellationToken ct)
    {
        if (isRunning) return;
        if(target == null) return;
        if (indicators.Count < 6) return;

        try
        {
            isRunning = true;

            int count = 12;
            float angleStep = 360f / count;
            Vector2[] dir = new Vector2[12];

            // dir 캐싱
            for(int i=0; i < count; i++)
            {
                float angle = i * angleStep;
                dir[i] = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            }

            // 인디케이터 연출
            for (int i = 0; i < count / 2; i++)
            {
                indicators[i].gameObject.SetActive(true);
                indicators[i].position = (Vector2)owner.transform.position - (dir[i] * rData.attackRange);
                indicators[i].rotation = Quaternion.Euler(0, 0, i * angleStep);
                indicators[i].localScale = new Vector3(rData.attackRange * 2, 0.2f, 1);
                indicatorRenderer[i].color = new Color(1, 0, 0, 0.2f); // 연한 빨강
            }

            // 선 딜레이
            await UniTask.Delay(TimeSpan.FromSeconds(rData.startDelay), cancellationToken: ct);

            // 3. 애니메이션 및 인디케이터 정리
            ShowAnim(rData.animTime, ct).Forget();
            IndicatorOffAll();

            await UniTask.Delay(TimeSpan.FromSeconds(rData.attackTime), cancellationToken: ct);

            //실제 공격 판정
            for (int i = 0; i < count; i++)
            {
                PoolManager.Instance.Instanciate(rData.id, new BulletArg
                {
                    id = rData.id,
                    atk = value,
                    dir = dir[i],
                    speed = rData.speed,
                    pos = owner.transform.position
                });
            }

            //후 딜레이
            await UniTask.Delay(TimeSpan.FromSeconds(rData.endDelay), cancellationToken: ct);

            // 쿨타임 시작
            SetCooltime(rData.coolTime, ct).Forget();
        }
        catch (OperationCanceledException)
        {
            CleanUp();
        }
        finally
        {
            isRunning = false;
        }
    }

    private async UniTaskVoid ShowAnim(float animTime, CancellationToken ct)
    {
        anim.SetBool("IsAttack", true);

        await UniTask.Delay(TimeSpan.FromSeconds(animTime), cancellationToken: ct).SuppressCancellationThrow();

        if (anim != null) anim.SetBool("IsAttack", false);
    }

    

    private void CleanUp()
    {
        IndicatorOffAll();
        if (anim != null) anim.SetBool("IsAttack", false);
    }
}