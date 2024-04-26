using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveLoadFunctions : MonoBehaviour
{
    public static GameSaveLoadFunctions Instance;
    private void Awake()
    {
        Instance = this;
    }

    public MenuData menuData;
    public SaveFileData[] saveFileData;


    public void Start()
    {
        GameSaveData data = SaveAndLoadGame.LoadInfo();
        if (data != null)
        {
            LoadDataFromFile(data);
        }
    }


    public void LoadDataFromFile(GameSaveData data)
    {
        AudioManager.Instance.UpdateAudioValues(data.menuData);
        menuData = data.menuData;
    }

    public void SaveMenuData(MenuData data)
    {
        menuData = data;
    }
    public void SaveSaveFileData(SaveFileData[] data)
    {
        saveFileData = data;
    }

    public void SaveDataToFile()
    {
        SaveAndLoadGame.SaveInfo(this);
    }
}
