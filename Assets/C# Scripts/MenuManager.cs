using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    private void Awake()
    {
        Instance = this;
    }





    public GameObject pauseMenuObj;
    public GameObject[] settingsMenus;

    public bool paused;
    public bool disableControl;

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (disableControl == false && ctx.performed && SelectionManager.Instance.isPlacingTower == false && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseRestartGame();
        }
    }

    
    public void PauseRestartGame()
    {
        if (disableControl == false)
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
    }
    public void RestartGame()
    {
        if (disableControl == false)
        {
            paused = false;

            Time.timeScale = 1;
            pauseMenuObj.SetActive(false);
            foreach (GameObject g in settingsMenus)
            {
                g.SetActive(false);
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("bennie UI");
    }
}
