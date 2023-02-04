using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    void Awake()
    {
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
    public void Play (string name)
        {
            Sound ps = Array.Find(sounds, AudioSource <= AudioSource.name == name);

            
            //ps.source.Play();
        }
}
