using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPop : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    public GameObject PopUpText;
    public GameObject PopDownText;

    public void Start()
    {
        PopUpText.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PopUpText.SetActive(true);
        PopDownText.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopUpText.SetActive(false);
        PopDownText.SetActive(true);
    }
}
