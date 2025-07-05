using System;
using UnityEngine;
using DG.Tweening;

public class GhostBoom : MonoBehaviour
{
    [Header("이벤트 필터링")]
    [Tooltip("이 컴포넌트가 반응할 GhostType을 지정")]
    public GhostType ghostType;
    public GhostUITransition transition;

    [Header("폭발 이펙트")]
    [Tooltip("폭발 시 인스펙터에 할당할 파티클 프리팹")]
    public ParticleSystem explosionPrefab;

    [Header("애니메이션 설정")]
    public float rotateDuration1 = 2f;    // 1단계 회전 지속 시간
    public float rotateShrinkDuration = 1f;  // 2단계 회전+축소 지속 시간
    public float explosionDelay = 2f;     // 폭발 후 대기 시간

    private void OnEnable()
    {
        GameManager.instance.onGestureFinalDone += OnGestureFinalDone;
    }

    private void OnDisable()
    {
        GameManager.instance.onGestureFinalDone -= OnGestureFinalDone;
    }

    private void OnGestureFinalDone(GhostType doneType)
    {
        // 자신이 지정된 타입이 아닐 땐 무시
        if (doneType != ghostType) return;
        PlayBoomSequence();
    }

    private void PlayBoomSequence()
    {
        // DOTween 시퀀스 생성
        Sequence seq = DOTween.Sequence();

        // 1) 2초 동안 360도 회전
        seq.Append(transform
            .DORotate(new Vector3(0, 360, 0), rotateDuration1, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
        );

        // 2) 1초 동안 축소하면서 동시에 360도 회전
        seq.Append(transform
            .DOScale(Vector3.zero, rotateShrinkDuration)
            .SetEase(Ease.InQuad)
        );
        seq.Join(transform
            .DORotate(new Vector3(0, 360, 0), rotateShrinkDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
        );

        // 3) 축소가 끝나는 즉시 폭발 파티클 인스턴스화
        seq.AppendCallback(() =>
        {
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        });

        // 4) 폭발 이펙트가 재생될 시간을 확보
        seq.AppendInterval(explosionDelay);

        // 5) 완료 시 오브젝트 파괴
        seq.OnComplete(() => { Destroy(gameObject);  transition.PlayTransition(); });
    }
}
