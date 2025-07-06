using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class GhostUITransition : MonoBehaviour
{
    // UI
    Canvas canvas;
    CanvasGroup cg;

    [Header("Hand References")]
    public GameObject handOld;
    public GameObject handNew;

    [Header("Music Objects")]
    public GameObject oldMusicObject;   // ↓ AudioSource 담긴 오브젝트
    public GameObject newMusicObject;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float audioTargetVolume = 1f;

    Coroutine cameraAssignCoroutine;

    // 내부에서 바로 쓰기 좋게 캐싱
    AudioSource oldSource;
    AudioSource newSource;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        cg = GetComponent<CanvasGroup>();
        canvas.gameObject.SetActive(false);
        cg.alpha = 0f;

        // GameObject → AudioSource 캐싱
        oldSource = oldMusicObject.GetComponent<AudioSource>();
        newSource = newMusicObject.GetComponent<AudioSource>();

        // 시작 상태
        oldSource.volume = 0f; oldSource.playOnAwake = false; oldSource.Pause();
        newSource.volume = 0f; newSource.playOnAwake = false; newSource.Pause();

        StartAssignCameraRoutine();
    }

    public void PlayTransition()
    {
        // (1) UI 보여주기
        canvas.gameObject.SetActive(true);
        cg.alpha = 0f;

        // (2) 어떤 소스 쓰길 결정
        AudioSource cur = handOld.activeInHierarchy ? oldSource : newSource;
        AudioSource prev = (cur == oldSource ? newSource : oldSource);

        // 이전 트윈·루틴 정리
        DOTween.Kill(cur);
        DOTween.Kill(prev);

        // (3) 새 소스 재생 준비
        cur.volume = 0f;
        cur.Play();

        // (4) 페이드 시퀀스
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);

        // UI & 음악 동시 페이드인
        seq.Append(cg.DOFade(1f, fadeDuration))
           .Join(cur.DOFade(audioTargetVolume, fadeDuration));

        // 전환 콜백: 핸드 오브젝트 교체
        seq.AppendCallback(() =>
        {
            handOld.SetActive(false);
            handNew.SetActive(true);
            StartAssignCameraRoutine();
        });

        // UI & 음악 동시 페이드아웃
        seq.Append(cg.DOFade(0f, fadeDuration))
           .Join(cur.DOFade(0f, fadeDuration));

        // 완료 시 정리
        seq.OnComplete(() =>
        {
            cur.Pause();
            canvas.gameObject.SetActive(false);
        });

        // 이전에 재생 중이던 소스도 멈춰두기
        prev.Pause();
    }

    IEnumerator AssignRenderCameraWhenAvailable()
    {
        while (true)
        {
            if (Camera.main && Camera.main.gameObject.activeInHierarchy)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void StartAssignCameraRoutine()
    {
        if (cameraAssignCoroutine != null) StopCoroutine(cameraAssignCoroutine);
        cameraAssignCoroutine = StartCoroutine(AssignRenderCameraWhenAvailable());
    }
}
