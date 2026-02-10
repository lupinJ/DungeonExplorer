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
            // 1. 준비 단계: 칼을 뒤로 살짝 뺌 (Anticipation)
            // 부모 기준 -30도 정도로 빠르게 예비 동작
            await transform.DOLocalRotate(new Vector3(0, 0, -30f), 0.1f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(ct);

            // 2. 휘두르기 단계: 순식간에 180도 회전 (Action)
            // 0.12초만에 강력하게 휘두름
            await transform.DOLocalRotate(new Vector3(0, 0, 150f), 0.12f)
                .SetEase(Ease.OutExpo)
                .WithCancellation(ct);

            // 3. 판정 시점 (이 타이밍에 Player의 공격 판정 로직 실행)
            GameManager.Instance.player.OnHitCheck();

            // 4. 잔상 유지 및 복귀 단계 (Recovery)
            await UniTask.WaitForSeconds(0.15f, cancellationToken: ct);

            
            // 원래 각도(0도)로 부드럽게 복귀
            
            await transform.DOLocalRotate(Vector3.zero, 0.0f)
                .SetEase(Ease.InSine)
                .WithCancellation(ct);
        }
        catch (System.OperationCanceledException) { /* 공격 취소 처리 */ }
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
