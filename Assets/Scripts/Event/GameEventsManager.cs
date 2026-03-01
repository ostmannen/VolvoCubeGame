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
    public void BigLanch()
    {
        if (OnBigLanch != null)
        {
            OnBigLanch();
        }
    }
    public event Action<GameObject> OnDeath;
    public void Death(GameObject player)
    {
        if (OnDeath != null)
        {
            OnDeath(player);
        }
    }
    public event Action<GameObject> OnRespawnPlayer;
    public void RespawnPlayer(GameObject player)
    {
        if (OnRespawnPlayer != null)
        {
            OnRespawnPlayer(player);
        }
    }
}
