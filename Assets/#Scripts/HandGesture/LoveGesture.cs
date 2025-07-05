using UnityEngine;
using System;
using UnityEngine.Events;

public class LoveGesture : GestureRecognizerBase
{
    [Header("손 오브젝트 참조")]
    public Transform leftPalm;
    public Transform rightPalm;

    [Header("포즈 임계값")]
    [Tooltip("왼손 높이로부터 오른손이 얼마 이상 올라와야 인식 시작할지")]
    public float minHeightAbove = 0.1f;

    // 내부 상태
    private bool poseStarted = false;
    private float prevAngle = 0f;           // 지난 프레임의 각도(도)
    private float cumulativeAngle = 0f;     // 누적 회전 각도(도, 음수 방향 누적)
    private int completedCircles = 0;       // 완성된 바퀴 수

    protected override void Recognize()
    {
        // 1) 포즈 시작 조건: 오른손이 왼손보다 충분히 위에 있을 때
        float heightDiff = rightPalm.position.y - leftPalm.position.y;
        if (!poseStarted)
        {
            if (heightDiff >= minHeightAbove)
            {
                poseStarted = true;
                // 초기 각도 세팅
                prevAngle = GetCurrentAngle();
                cumulativeAngle = 0f;
                completedCircles = 0;
                Debug.Log("Twist Pose 시작");
            }
            else return; // 아직 포즈 불충분
        }

        // 2) 회전 각도 계산
        float currentAngle = GetCurrentAngle();
        // 각도 차이 (시계 방향으로 돌면 negative)
        float delta = Mathf.DeltaAngle(prevAngle, currentAngle);
        // 시계 방향(negative delta)만 누적
        if (delta < 0f)
            cumulativeAngle += delta;
        prevAngle = currentAngle;

        // 3) 완성된 바퀴 수 계산
        int circles = Mathf.FloorToInt(-cumulativeAngle / 360f);
        if (circles > completedCircles)
        {
            completedCircles = circles;
            Debug.Log($"Twist 바퀴 완성: {completedCircles} 회");
        }

        // 4) 2바퀴 이상 돌았으면 제스처 완료
        if (completedCircles >= 2)
        {
            CompleteGesture();
        }
    }

    /// <summary>
    /// 오른손 위치를 왼손 기준으로 평면(수평) 위에서 atan2로 각도(도) 반환 [0,360)
    /// </summary>
    private float GetCurrentAngle()
    {
        Vector3 dir = rightPalm.position - leftPalm.position;
        // 수평면 투영
        Vector2 proj = new Vector2(dir.x, dir.z).normalized;
        float rad = Mathf.Atan2(proj.y, proj.x); // -π~π
        float deg = rad * Mathf.Rad2Deg;
        return (deg + 360f) % 360f;
    }

    protected override void ResetGesture()
    {
        base.ResetGesture();
        poseStarted = false;
        cumulativeAngle = 0f;
        completedCircles = 0;
        prevAngle = 0f;
    }
}
