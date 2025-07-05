using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostType
{
    ThankYou,
    Love,
    Hello,
    Help,
    Play
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<GhostType> onGestureDone;

    public void GestureDone(GhostType ghost)    //제스처 성공시
    {
        if (onGestureDone != null)
            onGestureDone(ghost);
    }

    public event Action<GhostType> onGestureFinalDone;

    public void GestureFinalDone(GhostType ghost)    //제스처 3번 성공시
    {
        if (onGestureFinalDone != null)
            onGestureFinalDone(ghost);
    }
}
