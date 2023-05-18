using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Audio
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup output;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
    public string[] level;

    private AudioSource source;

    public void InitSource(AudioSource source)
    {
        this.source = source;
        this.source.clip = this.clip;
        this.source.outputAudioMixerGroup = this.output;
        this.source.volume = this.volume;
        this.source.pitch = this.pitch;
        this.source.loop = this.loop;
    }
}
