using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    public static SliderManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Slider[] sliders;
}
