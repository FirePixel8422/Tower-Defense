using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public float scrollSpeed;

    public Resolution[] resolutions;
    public List<Resolution> filterdResolutionList;

    private int cresolutionIndex;
    public RefreshRate cRefreshRate;



    public TextMeshProUGUI fullscreenButtonText;

    public void ChangeFullScreenState()
    {
        bool newState = !Screen.fullScreen;
        Screen.fullScreen = newState;

        fullscreenButtonText.text = newState ? "Go Windowed" : "Go Fullscreen";
    }

    private void Start()
    {
        fullscreenButtonText.text = Screen.fullScreen ? "Go Windowed" : "Go Fullscreen";


        resolutions = Screen.resolutions;
        filterdResolutionList = new List<Resolution>();

        dropdown.ClearOptions();
        cRefreshRate = Screen.currentResolution.refreshRateRatio;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.Equals(cRefreshRate))
            {
                filterdResolutionList.Add(resolutions[i]);
            }
        }


        List<string> options = new List<string>();
        for (int i = 0; i < filterdResolutionList.Count; i++)
        {
            float refreshRate = (float)filterdResolutionList[i].refreshRateRatio.numerator / filterdResolutionList[i].refreshRateRatio.denominator;

            string resolutionOption = filterdResolutionList[i].width + "x" + filterdResolutionList[i].height + " " + refreshRate + "Hz";

            options.Add(resolutionOption);
        }
        filterdResolutionList.Reverse();
        options.Reverse();

        dropdown.AddOptions(options);

        for (int i = 0; i < filterdResolutionList.Count; i++)
        {
            if (filterdResolutionList[i].width == Screen.width && filterdResolutionList[i].height == Screen.height)
            {
                cresolutionIndex = i;
                break;
            }
        }
        
        dropdown.value = cresolutionIndex;
        dropdown.captionText.text = Screen.width + "x" + Screen.height + " " + Screen.currentResolution.refreshRateRatio + "Hz";

        dropdown.RefreshShownValue();
    }
  


    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filterdResolutionList[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
