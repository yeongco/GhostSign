using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GhostBoom : MonoBehaviour
{
    public GhostUITransition fader;

    // Start is called before the first frame update
    public void OnEnable()
    {
        GameManager.instance.onGestureFinalDone += GhostFadeOut;
    }
    public void OnDisable()
    {
        GameManager.instance.onGestureFinalDone -= GhostFadeOut;
    }

    public void GhostFadeOut(GhostType ghost)
    {
        //gameObject  //2초에 걸쳐 터지는 이벤트 (파티클 출력까지 4초 5초 걸린다고 침) (닷트윈)
        Invoke("PlayTransition", 5f);
    }

    void PlayTransition()
    {
        fader.PlayTransition();
    }
}
