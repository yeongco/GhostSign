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
    public float rotateDuration1 = 2f;
    public float rotateShrinkDuration = 1f;
    public float explosionDelay = 2f;  // 이 시간만큼 대기

    [Header("사운드 설정")]
    [Tooltip("Inspector에 AudioSource 컴포넌트를 넣고, Clip에 폭발 사운드를 할당하세요.")]
    public AudioSource audioSource;

    void Awake()
    {
        // AudioSource가 할당되지 않았다면 동일 오브젝트에서 가져오기
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = 1f;   // 필요 시 조정
    }

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
        if (doneType != ghostType) return;
        PlayBoomSequence();
    }

    private void PlayBoomSequence()
    {
        Sequence seq = DOTween.Sequence();

        // 1) 360도 회전
        seq.Append(transform
            .DORotate(new Vector3(0, 360, 0), rotateDuration1, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
        );

        // 2) 축소 & 회전
        seq.Append(transform
            .DOScale(Vector3.zero, rotateShrinkDuration)
            .SetEase(Ease.InQuad)
        );
        seq.Join(transform
            .DORotate(new Vector3(0, 360, 0), rotateShrinkDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
        );

        // 3) 폭발 이펙트
        seq.AppendCallback(() =>
        {
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        });

        // 4) 파티클 재생 시간 확보
        seq.AppendInterval(explosionDelay);

        // 5) 완료 시: UI 전환, 사운드 재생, 클립 길이만큼 대기 후 파괴
        seq.OnComplete(() =>
        {
            transition.PlayTransition();

            if (audioSource.clip != null)
            {
                audioSource.Play();
                // 스케일이 0이라 시각적으로는 이미 사라졌으므로, 소리만 재생 후 파괴
                Destroy(gameObject, audioSource.clip.length);
            }
            else
            {
                Destroy(gameObject);
            }
        });
    }
}
