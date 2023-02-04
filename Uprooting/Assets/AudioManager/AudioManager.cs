using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    public Sound[] sounds;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        foreach (Sound s in sounds)
        {
            s.source        = gameObject.AddComponent<AudioSource>();
            s.source.clip   = s.clip;

            s.source.name   = s.name;

            s.source.volume = s.volume;
            s.source.pitch  = s.pitch;

            s.source.loop   = s.loop;
        }
    }   

    public void PlayAudio (string name)
    {
            Sound ps = Array.Find(sounds, sound => sound.name == name);
            
            ps.source.Play();
    }
    public void StopAudio (string name)
    {
        Sound ps = Array.Find(sounds, sound => sound.name == name);

        // ps.source.Stop();
    }
    public void PauseAudio (string name)
    {
        Sound ps = Array.Find(sounds, sound => sound.name == name);

        ps.source.Pause();
    }
}
