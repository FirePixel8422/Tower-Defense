using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenuObj;

    public bool paused;

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PauseRestartGame();
        }
    }


    public void PauseRestartGame()
    {
        paused = !paused;

        Time.timeScale = paused ? 0 : 1;
        pauseMenuObj.SetActive(paused);
    }
}
