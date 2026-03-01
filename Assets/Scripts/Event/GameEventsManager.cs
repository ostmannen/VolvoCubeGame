using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }
    public void Awake()
    {
        instance = this;
    }
    public event Action OnBigLanch;
    public void NpcInteract()
    {
        if (OnBigLanch != null)
        {
            OnBigLanch();
        }
    }
}
