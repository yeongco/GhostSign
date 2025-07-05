using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class GhostUITransition : MonoBehaviour
{
    private Canvas canvas;         // 이 컴포넌트의 Canvas
    private CanvasGroup cg;        // 이 컴포넌트의 CanvasGroup

    public GameObject handOld;
    public GameObject handNew;

    public float fadeDuration = 1f;
    public float retryInterval = 0.5f;  // 카메라 탐색 주기 (초)

    private Coroutine cameraAssignCoroutine;

    void Start()
    {
        // 컴포넌트 캐싱
        canvas = GetComponent<Canvas>();
        cg     = GetComponent<CanvasGroup>();

        // 초기 투명 상태
        cg.alpha = 0f;

        // 최초 실행 시 카메라 할당 시도
        StartAssignCameraRoutine();
    }

    public void PlayTransition()
    {
        // 1) 페이드 준비
        cg.alpha = 0f;

        // 2) 시퀀스 생성
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(cg.DOFade(1f, fadeDuration));       // (a) 투명→불투명
        seq.AppendCallback(() =>
        {
            // (b) 오브젝트 전환 및 위치 설정
            handOld.SetActive(false);
            handNew.SetActive(true);

            // 카메라가 꺼졌을 수 있기 때문에 재할당 코루틴 재시작
            StartAssignCameraRoutine();
        });
        seq.Append(cg.DOFade(0f, fadeDuration));       // (c) 불투명→투명
        seq.OnComplete(() =>
        {
            // 끝나면 Canvas 비활성화
            canvas.gameObject.SetActive(false);
        });
    }

    private void StartAssignCameraRoutine()
    {
        // 이미 실행 중이면 중단 후 재시작
        if (cameraAssignCoroutine != null)
            StopCoroutine(cameraAssignCoroutine);

        cameraAssignCoroutine = StartCoroutine(AssignRenderCameraWhenAvailable());
    }

    private IEnumerator AssignRenderCameraWhenAvailable()
    {
        // 카메라가 유효해질 때까지 반복
        while (true)
        {
            // Camera.main이 존재하고 활성화된 경우 할당 후 종료
            if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
            {
                canvas.renderMode    = RenderMode.ScreenSpaceCamera;  
                canvas.worldCamera   = Camera.main;                  
                yield break;
            }
            // 다음 탐색까지 대기
            yield return new WaitForSeconds(retryInterval);
        }
    }
}
