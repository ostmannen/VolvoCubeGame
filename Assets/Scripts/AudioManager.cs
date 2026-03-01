using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
    }

    public Sound[] sounds;

    private Dictionary<string, Sound> soundLookup;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        soundLookup = new Dictionary<string, Sound>();

        foreach (var sound in sounds)
        {
            soundLookup[sound.id] = sound;
        }
       
    }
  
    public void Play(string id)
    {
        Sound s = soundLookup[id];
        audioSource.pitch = s.pitch;
        audioSource.PlayOneShot(s.clip, s.volume);
    }
}

