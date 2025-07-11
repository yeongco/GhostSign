using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpDownFourthAnim : MonoBehaviour
{
    [Header("이동 거리")]
    [Tooltip("현재 위치에서 위로 얼만큼 이동할지 (m)")]
    public float moveDistance = 1f;

    [Header("이동 시간")]
    [Tooltip("위로 올라가고 내려오는 데 걸리는 시간 (초)")]
    public float moveDuration = 0.2f;  // 한 번 올라가거나 내려가는 데 걸리는 시간

    [Header("대기 시간")]
    [Tooltip("4회 왕복 후 대기할 시간 (초)")]
    public float waitDuration = 0.5f;

    private Vector3 originalPos;  // 시작 위치 저장용

    void Start()
    {
        originalPos = transform.position;

        // ◆ 1) “한 번 올라갔다 내려오는”(왕복) 시퀀스 정의
        Sequence oneRound = DOTween.Sequence()
            .Append(transform
                .DOMoveY(originalPos.y + moveDistance, moveDuration)
                .SetEase(Ease.InOutSine))
            .Append(transform
                .DOMoveY(originalPos.y, moveDuration)
                .SetEase(Ease.InOutSine));
        // 4회 반복(왕복×4)
        oneRound.SetLoops(4, LoopType.Restart);

        // ◆ 2) 전체를 감싸는 시퀀스: 왕복×4 → 대기 → 다시 반복
        Sequence fullCycle = DOTween.Sequence()
            .Append(oneRound)             // 4번 왕복
            .AppendInterval(waitDuration) // 잠시 쉬고
            .SetLoops(-1, LoopType.Restart); // 무한 반복
    }
}
