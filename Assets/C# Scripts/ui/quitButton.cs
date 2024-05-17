using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class quitButton : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("The game started");
    }

    public void loadscene (string sceneName)
    {
        SceneManager.LoadScene (sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("the application has quit");
    }
}
