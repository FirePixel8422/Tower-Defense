using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public float scrollSpeed;

    public Resolution[] resolutions;
    public List<Resolution> filterdResolutionList;

    private int cresolutionIndex;
    public RefreshRate cRefreshRate;


    private void Start()
    {
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

            if (filterdResolutionList[i].width == Screen.width && filterdResolutionList[i].height == Screen.height)
            {
                cresolutionIndex = i;
            }
        }
        options.Reverse();

        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }
  


    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filterdResolutionList[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
}
