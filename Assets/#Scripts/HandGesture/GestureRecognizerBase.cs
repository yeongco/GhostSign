using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HandTypeEvent : UnityEvent<GestureRecognizerBase.HandType> { }

public abstract class GestureRecognizerBase : MonoBehaviour
{
    public GhostType ghostType;
    public enum HandType { Left = 0, Right = 1 }

    protected bool[] handReady = new bool[2];
    [Header("제스처 공통 설정")]
    public float gestureTimeout = 1.5f;
    protected float startTime;
    public GameObject ImageCanvas;
    private int gestureDoneCount = 0;
    bool eventLoopDone = false;

    public void OnEnable()
    {
        GameManager.instance.onGestureDone += onGestureDone;
    }

    public void OnDisable()
    {
        GameManager.instance.onGestureDone -= onGestureDone;
    }
    // 상태 머신 초기화
    protected virtual void Awake()
    {
        startTime = 0f;
    }

    // 매 프레임 Update
    void Update()
    {
        // 시간 초과 시 초기화
        if (startTime > 0f && Time.time - startTime > gestureTimeout)
            ResetGesture();

        // 자식 클래스에서 구체적 인식 수행
        Recognize();
    }

    // 제스처 인식을 구현할 추상 메서드
    protected abstract void Recognize();

    // 제스처 완료 콜백
    protected void CompleteGesture()
    {
        if (eventLoopDone)
            return;
        eventLoopDone = true;
        OnGestureDetected();
        Invoke("ResetGesture", 0.3f);
    }

    // 제스처 초기화
    protected virtual void ResetGesture()
    {
        eventLoopDone = false;
        startTime = 0f;
    }

    // 외부에서 오버라이드 가능한 완료 이벤트
    protected virtual void OnGestureDetected()
    {
        GameManager.instance.GestureDone(ghostType);
        Debug.Log($"[{name}] 제스처 인식 완료!");
    }

    public void IsLeftHandGestureOk()
    {
        handReady[0] = true;
        Debug.Log("왼손 준비 OK");
    }

    public void IsRightHandGestureOk()
    {
        handReady[1] = true;
        Debug.Log("오른손 준비 OK");
    }

    /// <summary>
    /// 지정된 손 준비 상태를 해제할 때 호출
    /// </summary>
    public void IsLeftHandGestureCancel()
    {
        handReady[0] = false;
        Debug.Log("왼손 준비 취소");
    }

    public void IsRightHandGestureCancel()
    {
        handReady[1] = false;
        Debug.Log("오른손 준비 취소");
    }

    public void onGestureDone(GhostType ghost)
    {
        // 내 GhostType이 아닐 때는 무시
        if (ghost != ghostType)
            return; 

        // 이미 3번 이상 처리했으면 무시
        if (gestureDoneCount >= 3)
            return;

        // 호출 횟수 증가 (1 → 첫 번째, 2 → 두 번째, 3 → 세 번째)
        gestureDoneCount++;

        // ImageCanvas의 자식 중 해당 인덱스 자식 활성화
        int childIndex = gestureDoneCount - 1; // 0,1,2
        if (childIndex < ImageCanvas.transform.childCount)
        {
            GameObject child = ImageCanvas.transform.GetChild(childIndex).gameObject;
            child.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"ImageCanvas 자식 개수 부족: 필요한 인덱스 {childIndex}");
        }

        // 세 번째 신호일 때 최종 완료 이벤트 호출
        if (gestureDoneCount == 3)
        {
            GameManager.instance.GestureFinalDone(ghost);
        }
    }
}
