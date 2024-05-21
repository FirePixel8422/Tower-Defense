using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPop : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    public GameObject PopUpText;

    public void Start()
    {
        PopUpText.SetActive(false);
    }

    public void OnMouseOver()
    {
        PopUpText.SetActive(true);
        print("over");
    }

    public void OnMouseExit()
    {
        PopUpText.SetActive(false);
        print("exit");

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PopUpText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopUpText.SetActive(false);
    }
}
