using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public MenuData menuData;
    public SaveFileData[] saveFileData;

    public GameSaveData(GameSaveLoadFunctions p)
    {
        menuData = p.menuData;
        saveFileData = p.saveFileData;
    }
}


[System.Serializable]
public struct MenuData
{
    public float mouseSens;
    public float audioMaster;
    public float audioSFX;
    public float audioMusic;
}
public struct SaveFileData
{

}
