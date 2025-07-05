using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HandTypeEvent : UnityEvent<GestureRecognizerBase.HandType> { }

public abstract class GestureRecognizerBase : MonoBehaviour
{
    
    public enum HandType { Left = 0, Right = 1 }

    protected bool[] handReady = new bool[2];
    [Header("제스처 공통 설정")]
    public float gestureTimeout = 1.5f;
    protected float startTime;

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
        OnGestureDetected();
        ResetGesture();
    }

    // 제스처 초기화
    protected virtual void ResetGesture()
    {
        startTime = 0f;
    }

    // 외부에서 오버라이드 가능한 완료 이벤트
    protected virtual void OnGestureDetected()
    {
        
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
}
