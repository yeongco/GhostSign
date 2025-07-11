using UnityEngine;
using DG.Tweening;

public class CircularWobbleMotion : MonoBehaviour
{
    [Header("궤적 설정")]
    [Tooltip("원형 반지름")]
    public float radius = 1f;
    [Tooltip("한 바퀴 도는 데 걸리는 시간(초)")]
    public float cycleDuration = 4f;
    [Tooltip("원 궤적 분할 개수")]
    public int segments = 60;

    [Header("회전 흔들림")]
    [Tooltip("흔들림 강도(도)")]
    public Vector3 shakeStrength = new Vector3(5f, 5f, 5f);
    [Tooltip("흔들림 빈도(vibrato)")]
    public int vibrato = 10;
    [Tooltip("흔들림 무작위성(% 비율)")]
    [Range(0f,1f)] public float randomness = 0.2f;

    private Vector3 centerPos;

    void Start()
    {
        // 1) 중심 위치 저장
        centerPos = transform.position;

        // 2) 원형 경로 생성
        Vector3[] path = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float angle = (360f / segments) * i * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            path[i] = centerPos + new Vector3(x, 0f, z);
        }

        // 3) 궤적 이동 애니메이션
        transform
            .DOPath(path, cycleDuration, PathType.Linear)
            .SetOptions(false) // Y 고정
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        // 4) 회전 흔들림 애니메이션
        transform
            .DOShakeRotation(cycleDuration, shakeStrength, vibrato, randomness)
            .SetLoops(-1, LoopType.Restart);
    }
}
