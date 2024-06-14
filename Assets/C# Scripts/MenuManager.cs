using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenuObj;
    public GameObject[] settingsMenus;

    public bool paused;

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && SelectionManager.Instance.isPlacingTower == false && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseRestartGame();
        }
    }


    public void PauseRestartGame()
    {
        paused = !paused;

        Time.timeScale = paused ? 0 : 1;
        pauseMenuObj.SetActive(paused);

        if (paused == false)
        {
            foreach (GameObject g in settingsMenus)
            {
                g.SetActive(false);
            }
        }
    }
    public void RestartGame()
    {
        paused = false;

        Time.timeScale = 1;
        pauseMenuObj.SetActive(false);
        foreach(GameObject g in settingsMenus)
        {
            g.SetActive(false);
        }
    }

    public void LoadMainMenu()
    {

    }
}
