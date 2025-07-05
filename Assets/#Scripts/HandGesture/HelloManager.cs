using UnityEngine;

public class HelloGesture : GestureRecognizerBase
{
    [Header("손 오브젝트 참조")]
    public Transform leftPalm;
    public Transform rightPalm;

    [Header("임계값")]
    public float dropDistance = 0.1f;  // 시작 높이 대비 하강 거리 (m)
    public float dropSpeed    = 0.2f;  // 하강 속도 임계값 (m/s)

    // 내부 상태
    private bool   poseOK         = false;
    private int    downCount      = 0;
    private bool   waitingForRise = false;
    private Vector3 startLeftPos, startRightPos;
    private Vector3 prevLeftPos, prevRightPos;

    protected override void Recognize()
    {
        // 1) 포즈 준비
        if (!poseOK && handReady[0] && handReady[1])
        {
            poseOK         = true;
            downCount      = 0;
            waitingForRise = false;
            startLeftPos   = leftPalm.position;
            startRightPos  = rightPalm.position;
            prevLeftPos    = startLeftPos;
            prevRightPos   = startRightPos;
            startTime      = Time.time;
        }
        if (!poseOK) return;

        // 타임아웃 처리
        if (startTime > 0f && Time.time - startTime > gestureTimeout)
        {
            ResetGesture();
            return;
        }

        // 2) 프레임별 움직임 계산
        Vector3 curLeft   = leftPalm.position;
        Vector3 curRight  = rightPalm.position;
        float  leftDy     = prevLeftPos.y   - curLeft.y;   // 양수일수록 아래로
        float  rightDy    = prevRightPos.y  - curRight.y;
        float  leftDist   = startLeftPos.y  - curLeft.y;
        float  rightDist  = startRightPos.y - curRight.y;
        float  leftSpeed  = leftDy  / Time.deltaTime;
        float  rightSpeed = rightDy / Time.deltaTime;

        prevLeftPos  = curLeft;
        prevRightPos = curRight;

        // 3) 하강 감지 로직
        if (!waitingForRise)
        {
            // 두 손 모두 충분히 내려오고 속도 조건 만족 시 첫/두번째 하강 카운트
            if (leftDist >= dropDistance && rightDist >= dropDistance &&
                leftSpeed >= dropSpeed  && rightSpeed >= dropSpeed)
            {
                downCount++;
                Debug.Log($"하강 감지 {downCount}/2");
                waitingForRise = true;      // 다음엔 “올라옴”을 기다림
            }
        }
        else
        {
            // 손이 거의 원위치로 올라오면 다시 하강 대기 상태로
            bool leftRise  = curLeft.y   >= startLeftPos.y - 0.02f;
            bool rightRise = curRight.y  >= startRightPos.y - 0.02f;
            if (leftRise && rightRise)
                waitingForRise = false;
        }

        // 4) 두 번 하강 완료 시 제스처 인식
        if (downCount >= 2)
        {
            CompleteGesture();
        }
    }

    protected override void ResetGesture()
    {
        base.ResetGesture();
        poseOK         = false;
        downCount      = 0;
        waitingForRise = false;
    }

    protected override void OnGestureDetected()
    {
        Debug.Log($"[{name}] 아래로 두 번 내리기 제스처 인식 완료!");
        GameManager.instance.GestureDone(ghostType);
    }
}
