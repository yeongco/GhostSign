using UnityEngine;
using DG.Tweening;

public class GhostUITransition : MonoBehaviour
{
    public CanvasGroup uiCanvas;          // �� ���� �̰� ��ü Canvas
    public GameObject handOld;
    public GameObject handNew;

    public float fadeDuration = 1f;

    public void PlayTransition()
    {
        // ȭ�� ���̵� �ƿ�
        uiCanvas.alpha = 0;
        uiCanvas.gameObject.SetActive(true);
        handOld.SetActive(false);

        uiCanvas.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            handNew.SetActive(true);
            handNew.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.4f;
        });
    }
}
