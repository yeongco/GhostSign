using UnityEngine;
using UnityEngine.Events;

public class HelpGestureFourTaps : GestureRecognizerBase
{
    [Header("손 오브젝트 참조")]
    public Transform leftPalm;
    public Transform rightPalm;

    [Header("임계값")]
    public float contactDistance = 0.05f;  // 접촉 거리 (m)
    public float contactSpeed    = 0.3f;   // 최소 속도 (m/s)
    public float hitTimeout      = 1f;     // 첫 타격 후 타임아웃 (초)

    [Header("네 번 두드리기")]
    public int requiredHits = 4;           // 필요 타격 수

    // 내부 상태
    private bool  poseOK       = false;
    private int   hitCount     = 0;
    private float lastHitTime  = 0f;
    private Vector3 prevRightPos;

    protected override void Recognize()
    {
        // 1) 포즈 준비 (양손이 OK 상태)
        if (!poseOK && handReady[0] && handReady[1])
        {
            poseOK        = true;
            hitCount      = 0;
            lastHitTime   = Time.time;
            prevRightPos  = leftPalm.position;
        }
        if (!poseOK) 
            return;

        // 2) 타임아웃 처리: 일정 시간 지나면 카운트 리셋
        if (hitCount > 0 && Time.time - lastHitTime > hitTimeout)
        {
            Debug.Log("타임아웃: 타격 카운트 리셋");
            hitCount     = 0;
            lastHitTime  = Time.time;
        }

        // 3) 타격 감지 시도
        TryDetectHit();

        // 4) 네 번 타격 완료 시 제스처 인식
        if (hitCount >= requiredHits)
        {
            CompleteGesture();
        }
    }

    private void TryDetectHit()
    {
        Vector3  cur      = leftPalm.position;
        float    dist     = Vector3.Distance(cur, rightPalm.position);
        float    speed    = Vector3.Distance(cur, prevRightPos) / Time.deltaTime;
        prevRightPos      = cur;

        // 접촉 + 속도 조건 만족 시 카운트 증가
        if (dist < contactDistance && speed > contactSpeed)
        {
            hitCount++;
            lastHitTime = Time.time;
            Debug.Log($"두드리기 감지 {hitCount}/{requiredHits}");
        }
    }

    protected override void ResetGesture()
    {
        base.ResetGesture();
        poseOK      = false;
        hitCount    = 0;
        lastHitTime = 0f;
    }

    protected override void OnGestureDetected()
    {
        Debug.Log($"[{name}] 네 번 두드리기 제스처 인식 완료!");
        // GameManager 호출 등 추가 로직
    }
}
