using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }


    //all audio in scene
    public List<AudioController> audioControllers;


    private void Start()
    {
        audioControllers = FindObjectsOfType<AudioController>().ToList();

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
        print("sceneloaded");
        yield return new WaitForEndOfFrame();

        Slider[] sliders = SliderManager.Instance.sliders;

        sliderMouseSens = sliders[0];
        sliderMaster = sliders[1];
        sliderSFX = sliders[2];
        sliderMusic = sliders[3];


        sliderMouseSens.value = menuData.mouseSens;
        sliderMaster.value = menuData.audioMaster;
        sliderSFX.value = menuData.audioSFX;
        sliderMusic.value = menuData.audioMusic;

        sliderMouseSens.onValueChanged.AddListener((value) => OnMouseSensChanged(value));
        sliderMaster.onValueChanged.AddListener((value) => OnMasterChanged(value));
        sliderSFX.onValueChanged.AddListener((value) => OnSFXChanged(value));
        sliderMusic.onValueChanged.AddListener((value) => OnMusicChanged(value));

        audioControllers = FindObjectsOfType<AudioController>(true).ToList();
        foreach (AudioController controller in audioControllers)
        {
            controller.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
            MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
        }

        if (CameraController.Instance != null)
        {
            CameraController.Instance.mouseSens = menuData.mouseSens;
        }
    }


    public Slider sliderMouseSens;
    public Slider sliderMaster;
    public Slider sliderSFX;
    public Slider sliderMusic;

    public MenuData menuData;


    public void UpdateAudioValues(MenuData data)
    {
        sliderMouseSens.value = data.mouseSens;
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

    public void OnMouseSensChanged(float f)
    {
        menuData.mouseSens = f;
        if (CameraController.Instance != null)
        {
            CameraController.Instance.mouseSens = menuData.mouseSens;
        }
        SaveAudioDataToFile();
    }
    public void OnMasterChanged(float f)
    {
        menuData.audioMaster = f;
        MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);

        foreach (AudioController controller in audioControllers)
        {
            controller.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
            MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
        }
        SaveAudioDataToFile();
    }
    public void OnSFXChanged(float f)
    {
        menuData.audioSFX = f;

        foreach (AudioController controller in audioControllers)
        {
            controller.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
            MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
        }
        SaveAudioDataToFile();
    }
    public void OnMusicChanged(float f)
    {
        menuData.audioMusic = f;
        MusicManager.Instance.UpdateVolume(menuData.audioMaster, menuData.audioSFX, menuData.audioMusic);
        SaveAudioDataToFile();
    }
}
