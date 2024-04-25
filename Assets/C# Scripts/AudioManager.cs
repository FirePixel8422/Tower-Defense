using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        Instance = this;
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
    }
}
