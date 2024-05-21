using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    //all audio in scene
    public AudioController[] audioControllers;


    private void Start()
    {
        audioControllers = FindObjectsOfType<AudioController>();

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneloaded;
    }

    private void OnSceneloaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Thomas Main")
        {
            MusicManager.Instance.ChangeMusicTrack(false, 0.5f);
        }
        MusicManager.Instance.StartCoroutine(SceneChangedDelay());
    }
    private IEnumerator SceneChangedDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return null;

        audioControllers = FindObjectsOfType<AudioController>();
        foreach (AudioController controller in audioControllers)
        {
            controller.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
            MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
        }
    }


    public Slider sliderMaster;
    public Slider sliderSFX;
    public Slider sliderMusic;

    public MenuData menuData;


    public void UpdateAudioValues(MenuData data)
    {
        sliderMaster.value = data.audioMaster;
        sliderSFX.value = data.audioSFX;
        sliderMusic.value = data.audioMusic;

        foreach (AudioController controller in audioControllers)
        {
            controller.UpdateVolume(data.audioMaster, data.audioSFX, data.audioMusic);
            MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
        }
    }

    public void SaveAudioDataToFile()
    {
        GameSaveLoadFunctions.Instance.SaveMenuData(menuData);
        GameSaveLoadFunctions.Instance.SaveDataToFile();
    }


    public void OnMasterChanged(float f)
    {
        menuData.audioMaster = f;
    }
    public void OnSFXChanged(float f)
    {
        menuData.audioSFX = f;
    }
    public void OnMusicChanged(float f)
    {
        menuData.audioMusic = f;
        MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
    }
}
