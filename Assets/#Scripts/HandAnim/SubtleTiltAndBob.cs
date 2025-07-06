using UnityEngine;
using DG.Tweening;

public class SubtleTiltAndBob : MonoBehaviour
{
    [Header("회전 기울기")]
    [Tooltip("X축에서 얼마나 기울일지 (도)")]
    public float tiltAngle = 5f;

    [Header("버브(상하 이동) 거리")]
    [Tooltip("현재 위치에서 얼마나 아래로 내려갈지 (m)")]
    public float bobDistance = 0.1f;

    [Header("사이클 시간")]
    [Tooltip("기울기 ↔ 복원 + 버브↔복원 한 사이클에 걸리는 시간 (초)")]
    public float cycleDuration = 1f;

    private Vector3 originalEuler;
    private Vector3 originalPos;

    void Start()
    {
        // 원래 각도·위치 저장
        originalEuler = transform.eulerAngles;
        originalPos   = transform.position;

        // 1) X축 기울기: originalEuler.x 에서 -tiltAngle 만큼 기울였다가 복원
        transform
            .DORotate(
                new Vector3(originalEuler.x - tiltAngle, 
                            originalEuler.y, 
                            originalEuler.z),
                cycleDuration / 2f, // 내려갈 때 절반 시간
                RotateMode.Fast
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // 2) 상하 버브: originalPos.y 에서 -bobDistance 만큼 내려갔다가 복원
        transform
            .DOMoveY(originalPos.y - bobDistance, cycleDuration / 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
