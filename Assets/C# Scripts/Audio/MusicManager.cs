using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static AudioController;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public AudioSource musicPlayer;
    public AudioSource musicPlayerAlt;

    public bool altMusicPlayerActive;

    public AudioClip mainMenuClip;
    public AudioClip[] battleFieldClips;
    private int clipIndex;

    private Coroutine queNextTrackCO;


    public void UpdateVolume(float main, float sfx, float music)
    {
        musicPlayer.volume = main;
        musicPlayer.volume *= music;
        musicPlayerAlt.volume = main;
        musicPlayerAlt.volume *= music;
    }


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        clipIndex = Random.Range(0, battleFieldClips.Length);
        ChangeMusicTrack(true, 0.5f);
    }

    public void ChangeMusicTrack(bool mainMenu, float fadeSpeed)
    {
        if(queNextTrackCO != null)
        {
            StopCoroutine(queNextTrackCO);
        }

        AudioClip clip = mainMenuClip;
        AudioClip queClip = mainMenuClip;

        if (mainMenu == false)
        {
            clip = battleFieldClips[clipIndex];

            clipIndex += 1;
            if(clipIndex >= battleFieldClips.Length)
            {
                clipIndex = 0;
            }
            queClip = battleFieldClips[clipIndex];
        }
        StartCoroutine(FadeChangeMusicTrack(clip, fadeSpeed));
        queNextTrackCO = StartCoroutine(QueNextTracktimer(queClip, queClip.length, mainMenu));
    }

    private IEnumerator QueNextTracktimer(AudioClip clip, float delay, bool mainMenu)
    {
        yield return new WaitForSeconds(delay - 0.5f);
        StartCoroutine(FadeChangeMusicTrack(clip, 0.5f));

        if (mainMenu == false)
        {
            clip = battleFieldClips[clipIndex];

            clipIndex += 1;
            if (clipIndex >= battleFieldClips.Length)
            {
                clipIndex = 0;
            }
        }
        queNextTrackCO = StartCoroutine(QueNextTracktimer(clip, clip.length, mainMenu));
    }

    private IEnumerator FadeChangeMusicTrack(AudioClip audioClip, float fadeSpeed)
    {
        AudioSource currentSource = altMusicPlayerActive ? musicPlayerAlt : musicPlayer;
        AudioSource altSource = altMusicPlayerActive ? musicPlayer : musicPlayerAlt;

        altSource.clip = audioClip;
        altSource.Play();

        while (currentSource.volume > 0)
        {
            currentSource.volume -= fadeSpeed * 0.05f;
            altSource.volume += fadeSpeed * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        currentSource.Stop();

        altMusicPlayerActive = !altMusicPlayerActive;
    }
}
