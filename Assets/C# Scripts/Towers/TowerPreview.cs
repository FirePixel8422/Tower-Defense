using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    public TowerCore towerPrefab;
    public int placementIndex;

    public float scrapCost;

    public bool locked;

    [HideInInspector]
    public DissolveController[] dissolves;

    [HideInInspector]
    public SpriteRenderer towerPreviewRenderer;
    private Color cColor;


    private void Start()
    {
        scrapCost = towerPrefab.towerUIData.buildCost;

        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
        towerPreviewRenderer.transform.localScale = Vector3.one * towerPrefab.range;

        dissolves = GetComponentsInChildren<DissolveController>();
        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetInt("_Preview", true ? 1 : 0);
        }
    }

    public void UpdateTowerPreviewColor(Color color)
    {
        if (color == cColor)
        {
            return;
        }
        cColor = color;
        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetColor("_PreviewColor", color);
        }
    }
}
