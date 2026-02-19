using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DashSkill : Skill
{
    public DashSkill(SkillDataSO data, SkillContext ctx) : base(data, ctx) { }

    public override async UniTask Activate(Transform target, int value, CancellationToken ct)
    {
        if (isRunning) return;

        // Mp 소모
        if (stat.Mp < data.mpCost)
            return;

        stat.Mp -= data.mpCost;
        isRunning = true;

        await UniTask.Delay(TimeSpan.FromSeconds(data.startDelay), cancellationToken: ct);

        anim.SetBool("IsDash", true);
        movement.Speed += data.attackRange;
        movement.IsLockon = true;
        stat.InvincibleAsync(0.2f, ct).Forget(); // 대쉬 무적

        await UniTask.Delay(TimeSpan.FromSeconds(data.animTime), cancellationToken: ct);

        movement.Speed -= data.attackRange;
        movement.IsLockon = false;
        movement.Dir = InputManager.Instance.MoveInput; // movement 갱신
        anim.SetBool("IsDash", false);

        await UniTask.Delay(TimeSpan.FromSeconds(data.endDelay), cancellationToken: ct);

        SetCooltime(data.coolTime, ct).Forget();
        isRunning = false;
    }
}
