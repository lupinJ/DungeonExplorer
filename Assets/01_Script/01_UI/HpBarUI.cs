using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUI : UIBase, IInItable
{
    
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI hpText;

    [SerializeField] Slider mpSlider;
    [SerializeField] TextMeshProUGUI mpText;

    CancellationTokenSource hpCts;
    CancellationTokenSource mpCts;

    public void Initialize(InitData data = default)
    {
        // hp, mp 이벤트 구독
        EventManager.Instance.Subscribe<PlayerStat.HpEvent, PointArg>(OnHpChanged);
        EventManager.Instance.Subscribe<PlayerStat.MpEvent, PointArg>(OnMpChanged);
    }

    private void OnHpChanged(PointArg arg)
    {
        float nextHp = (float)arg.current / arg.max;

        hpCts?.Cancel();
        hpCts?.Dispose();

        hpCts = new CancellationTokenSource();

        hpText.text = $"{arg.current}/{arg.max}";
        UpdateHpAsync(arg.current, nextHp, hpCts.Token).Forget();
    }

    private async UniTaskVoid UpdateHpAsync(int hp, float nextHp, CancellationToken ck)
    {
        try
        {
            await DOTween.To(() => hpSlider.value, x => hpSlider.value = x, nextHp, 0.5f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(ck);
        }
        catch (System.OperationCanceledException)
        {
            
        }
    }
    private void OnMpChanged(PointArg arg)
    {
        float nextMp = (float)arg.current / arg.max;

        mpCts?.Cancel();
        mpCts?.Dispose();

        mpCts = new CancellationTokenSource();

        mpText.text = $"{arg.current}/{arg.max}";
        UpdateMpAsync(arg.current, nextMp, mpCts.Token).Forget();
    }

    private async UniTaskVoid UpdateMpAsync(int mp, float nextMp, CancellationToken ck)
    {
        try
        {
            await DOTween.To(() => mpSlider.value, x => mpSlider.value = x, nextMp, 0.5f)
                .SetEase(Ease.OutQuad)
                .WithCancellation(ck);
        }
        catch (System.OperationCanceledException)
        {

        }
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<PlayerStat.HpEvent, PointArg>(OnHpChanged);
            EventManager.Instance.Unsubscribe<PlayerStat.MpEvent, PointArg>(OnMpChanged);
        }

        hpCts?.Cancel();
        hpCts?.Dispose();
        mpCts?.Cancel();
        mpCts?.Dispose();
    }

}
