using System;
using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range (0f, 1f)]
    public float volume;
    [Range (0.1f, 3f)]
    public float pitch;
    /* Optional, I dunno if it's needed:
    [Range (0f, 1f)]
    public float spacialBlend;
    [Range (-1f, 1f)]
    public float stereoPan;
    */
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
