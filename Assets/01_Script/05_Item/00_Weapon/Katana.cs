using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class Katana : Weapon
{
    GameObject parent;
    CancellationTokenSource attackCts;

    public override void Initialize(InitData data = default)
    {
        if(data is WeaponArg weapon)
        {
            Item = weapon.item;
            parent = weapon.parent;
            isAttack = false;
        }
    }

    public override void Attack()
    {
        if (isAttack) return; // 공격 중 중복 실행 방지

        attackCts?.Cancel();
        attackCts?.Dispose();
        attackCts = new CancellationTokenSource();

        PerformKatanaSwing(attackCts.Token).Forget();
    }

    private async UniTaskVoid PerformKatanaSwing(CancellationToken ct)
    {
        isAttack = true;

        try
        {
            // 준비, 칼을 뒤로 살짝 뺌
            await transform.DOLocalRotate(new Vector3(0, 0, -30f), 0.1f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(ct);

            // 휘두르기, 180도 회전
            await transform.DOLocalRotate(new Vector3(0, 0, 150f), 0.12f)
                .SetEase(Ease.OutExpo)
                .WithCancellation(ct);

            // 공격 판정
            GameManager.Instance.player.OnHitCheck();

            // 잔상 유지 및 복귀
            await UniTask.WaitForSeconds(0.15f, cancellationToken: ct);

            
            // 복귀
            await transform.DOLocalRotate(Vector3.zero, 0.0f)
                .SetEase(Ease.InSine)
                .WithCancellation(ct);
        }
        catch (System.OperationCanceledException) { }
        finally
        {
            isAttack = false;
        }
    }

    private void OnDisable()
    {
        attackCts?.Cancel();
        attackCts?.Dispose();
        attackCts = null;
    }
}
