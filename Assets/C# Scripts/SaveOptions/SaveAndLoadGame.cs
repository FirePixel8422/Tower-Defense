using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveAndLoadGame
{
    public static void SaveInfo(GameSaveLoadFunctions p)
    {
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GameData.MT";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameSaveData data = new GameSaveData(p);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Data Saved Correctly");
    }

    public static GameSaveData LoadInfo()
    {
        string path = Application.persistentDataPath + "/GameData.MT";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameSaveData data = formatter.Deserialize(stream) as GameSaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found yet in" + path);
            return null;
        }
    }
}
