using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpDownAnim : MonoBehaviour
{
    [Header("이동 거리")]
    [Tooltip("현재 위치에서 위로 얼만큼 이동할지 (m)")]
    public float moveDistance = 1f;

    [Header("이동 시간")]
    [Tooltip("위로 올라가고 내려오는 데 걸리는 시간 (초)")]
    public float moveDuration = 0.5f;

    [Header("대기 시간")]
    [Tooltip("한 사이클 후 대기할 시간 (초)")]
    public float waitDuration = 0.2f;

    private Vector3 originalPos;  // 시작 위치 저장용

    void Start()
    {
        // 1) 원위치 저장
        originalPos = transform.position;

        // 2) DOTween Sequence 구성
        Sequence seq = DOTween.Sequence();

        // 2-1) 원위치 → 위위치
        seq.Append(transform
            .DOMoveY(originalPos.y + moveDistance, moveDuration)
            .SetEase(Ease.InOutSine));

        // 2-2) 위위치 → 원위치
        seq.Append(transform
            .DOMoveY(originalPos.y, moveDuration)
            .SetEase(Ease.InOutSine));

        // 2-3) 대기
        seq.AppendInterval(waitDuration);

        // 3) 무한 반복 설정
        seq.SetLoops(-1, LoopType.Restart);
    }
}
