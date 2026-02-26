using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class JumpAttack : Skill
{
    private JumpAttackSO jData;
    private Monster monster;
    readonly int playerLayer;

    public JumpAttack(SkillDataSO data, SkillContext ctx) : base(data, ctx)
    {
        jData = data as JumpAttackSO;
        monster = owner as Monster;
        playerLayer = LayerMask.GetMask("Player");
    }

    public override async UniTask Activate(Transform target, int value, CancellationToken ct)
    {
        if (isRunning) return;
        if (target == null) return;
        if (indicators.Count < 1) return;

        try
        {
            isRunning = true;

            // 점프 애니메이션
            anim.SetBool("IsUp", true);
            await UniTask.Delay(TimeSpan.FromSeconds(jData.upAnimTime), cancellationToken: ct);

            // 끝나면 잠깐 사라짐
            var sprite = owner.GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.enabled = false;
            stat.InvincibleAsync(jData.upDelay + jData.startDelay, ct).Forget();
            if(monster != null) monster.SetHpBarActive(false);

            // 잠시 날아다님
            await UniTask.Delay(TimeSpan.FromSeconds(jData.upDelay), cancellationToken: ct);

            // 인디케이터 연출, 이때의 player위치가 착지 지점
            Vector2 attackPosition = target.position;

            indicators[0].gameObject.SetActive(true);
            indicators[0].position = attackPosition;
            indicators[0].localScale = new Vector3(jData.attackRadius * 2, jData.attackRadius * 2, 1);
            indicatorRenderer[0].color = new Color(1, 0, 0, 0.2f);

            // 선 딜레이
            await UniTask.Delay(TimeSpan.FromSeconds(jData.startDelay), cancellationToken: ct);

            // 내려찍기 애니메이션
            owner.transform.position = attackPosition;
            if (sprite != null) sprite.enabled = true; 
            anim.SetBool("IsUp", false);

            // 내려찍는 애니메이션 및 인디케이터 정리
            ShowAnim("IsDown", jData.animTime, ct).Forget();
            if (monster != null) monster.SetHpBarActive(true);
            IndicatorOffAll();

            await UniTask.Delay(TimeSpan.FromSeconds(jData.attackTime), cancellationToken: ct);

            // 실제 공격 판정 (공격지점 공격)
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPosition, jData.attackRadius, playerLayer);
            foreach (var hit in hitTargets)
            {
                if (hit.isTrigger == true && hit.TryGetComponent<IHitable>(out var damageable))
                {
                    damageable.Hit(value);
                }
            }

            //후 딜레이
            await UniTask.Delay(TimeSpan.FromSeconds(jData.endDelay), cancellationToken: ct);

            // 쿨타임 시작
            SetCooltime(jData.coolTime, ct).Forget();
        }
        catch (OperationCanceledException)
        {
            CleanUp();
        }
        finally
        {
            if(owner != null)
            {
                var sprite = owner.GetComponent<SpriteRenderer>();
                if (sprite != null) sprite.enabled = true;
            }
            
            isRunning = false;
        }
    }

    private void CleanUp()
    {
        IndicatorOffAll();
        if (anim != null)
        {
            anim.SetBool("IsDown", false);
            anim.SetBool("IsUp", false);
        }
    }
}
