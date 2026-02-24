using Cysharp.Threading.Tasks;
using DG.Tweening; // DOTween 네임스페이스
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FadePanelUI : UIBase
{
    [SerializeField] private Image fadeImage;
    private Tween fadeTween;

    private void Awake()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        fadeImage.raycastTarget = true;
    }

    public async UniTask FadeAsync(float targetAlpha, float duration)
    {
        fadeImage.raycastTarget = true;

        if (fadeTween != null && fadeTween.IsActive())
        {
            fadeTween.Kill();
        }

        // Image의 Alpha값 제어
        fadeTween = fadeImage.DOFade(targetAlpha, duration)
            .SetUpdate(true) // TimeScale 영향x
            .SetEase(Ease.Linear); 

        await fadeTween.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        if(targetAlpha <= 0f)
            fadeImage.raycastTarget = false;

    }

    private void OnDestroy()
    {
        fadeTween?.Kill();
    }
}