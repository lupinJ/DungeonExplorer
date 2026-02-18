using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // 추적 대상
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Follow Settings")]
    [SerializeField] private float followDuration = 0.2f; // 카메라가 도달하는 시간
    [SerializeField] private Ease followEase = Ease.Linear;

    private CancellationTokenSource cts;

    private void Start()
    {
        cts = new CancellationTokenSource();
        FollowLoop(cts.Token).Forget();
    }

    private async UniTaskVoid FollowLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (target != null)
            {
                Vector3 targetPos = target.position + offset;

                await transform.DOMove(targetPos, followDuration)
                    .SetEase(followEase)
                    .SetLink(gameObject)
                    .ToUniTask(TweenCancelBehaviour.Kill, ct);
            }
            else
            {
                // 타겟이 없으면 다음 프레임 대기
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
    }

    public void SetTarget(Transform target) { this.target = target; }

    // 외부에서 카메라 흔들기 연출을 호출할 때 사용
    public async UniTask ShakeCamera(float duration = 0.5f, float strength = 0.5f)
    {
        // 카메라 흔들기 도중에는 추적 루프와 간섭이 생길 수 있으므로 
        // DOComplete로 이전 트윈 정리 후 실행하거나 전용 연출 레이어 사용
        await transform.DOShakePosition(duration, strength)
            .SetLink(gameObject)
            .ToUniTask();
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
