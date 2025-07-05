using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayGesture : GestureRecognizerBase
{
    [Header("손 오브젝트 참조")]
    public Transform leftPalm;
    public Transform rightPalm;

    [Header("포즈 임계값")]
    [Tooltip("왼손 높이로부터 오른손이 얼마 이상 올라와야 인식 시작할지")]
    public float minHeightAbove = 0.1f;

    [Header("회전 바퀴 수")]
    [Tooltip("시계 방향으로 몇 바퀴 돌면 인식 완료할지")]
    public int requiredCircles = 3;  // 3바퀴로 변경

    // 내부 상태
    private bool  poseStarted       = false;
    private float prevAngle         = 0f;   // 지난 프레임의 각도(도)
    private float cumulativeAngle   = 0f;   // 누적 회전 각도(도, 음수 방향 누적)
    private int   completedCircles  = 0;    // 완성된 바퀴 수

    protected override void Recognize()
    {
        // 1) 포즈 시작 조건
        float heightDiff = rightPalm.position.y - leftPalm.position.y;
        if (!poseStarted)
        {
            if (heightDiff >= minHeightAbove)
            {
                poseStarted      = true;
                prevAngle        = GetCurrentAngle();
                cumulativeAngle  = 0f;
                completedCircles = 0;
                startTime        = Time.time;
                Debug.Log("Twist Pose 시작");
            }
            else return;
        }

        // 2) 시간 초과 리셋
        if (startTime > 0f && Time.time - startTime > gestureTimeout)
        {
            ResetGesture();
            return;
        }

        // 3) 회전 각도 누적
        float currentAngle = GetCurrentAngle();
        float delta        = Mathf.DeltaAngle(prevAngle, currentAngle);
        if (delta < 0f)  // 시계 방향 회전일 때만
            cumulativeAngle += delta;
        prevAngle = currentAngle;

        // 4) 완성된 바퀴 수 업데이트
        int circles = Mathf.FloorToInt(-cumulativeAngle / 360f);
        if (circles > completedCircles)
        {
            completedCircles = circles;
            Debug.Log($"Twist 바퀴 완성: {completedCircles} 회");
        }

        // 5) requiredCircles 바퀴 달성 시 제스처 완료
        if (completedCircles >= requiredCircles)
        {
            CompleteGesture();
        }
    }

    /// <summary>
    /// 오른손 위치를 왼손 기준으로 평면 위에서 atan2로 각도(도) 반환 [0,360)
    /// </summary>
    private float GetCurrentAngle()
    {
        Vector3 dir  = rightPalm.position - leftPalm.position;
        Vector2 proj = new Vector2(dir.x, dir.z).normalized;
        float   rad  = Mathf.Atan2(proj.y, proj.x);
        float   deg  = rad * Mathf.Rad2Deg;
        return (deg + 360f) % 360f;
    }

    protected override void ResetGesture()
    {
        base.ResetGesture();
        poseStarted      = false;
        cumulativeAngle  = 0f;
        completedCircles = 0;
        prevAngle        = 0f;
    }
}
