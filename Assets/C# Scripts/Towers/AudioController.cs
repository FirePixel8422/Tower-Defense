using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public AudioSource[] audioSources;

    public Audio_Type audioType;
    public enum Audio_Type
    {
        Master,
        SoundEffects,
        Music
    };

    public AudioClip[] clips;
    private int clipIndex;
    public OrderMode clipOrder;
    public enum OrderMode
    {
        InOrder,
        FullyRandom
    };


    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
    }


    public void UpdateVolume(float main, float sfx, float music)
    {
        foreach (AudioSource source in audioSources)
        {
            source.volume = main;
            if (audioType == Audio_Type.SoundEffects)
            {
                source.volume *= sfx;
            }
            if (audioType == Audio_Type.Music)
            {
                source.volume *= music;
            }
        }
    }

    public void Play()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source.isPlaying)
            {
                continue;
            }

            if (clipOrder == OrderMode.FullyRandom)
            {
                int r = Random.Range(0, clips.Length);
                source.clip = clips[r];
            }
            else
            {
                source.clip = clips[clipIndex];
                clipIndex += 1;
                if (clipIndex >= clips.Length)
                {
                    clipIndex = 0;
                }
            }
            source.Play();
            return;
        }
        print("too little audiosources on: " +gameObject.name);
    }
}
