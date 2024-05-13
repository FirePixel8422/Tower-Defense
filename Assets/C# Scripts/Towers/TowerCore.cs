using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    public int amountOfDissolves;
    public int cDissolves;

    public bool towerCompleted;

    [HideInInspector]
    public SpriteRenderer towerPreviewRenderer;

    public MagicType[] magicType;
    public float rotSpeed;
    public Transform rotPoint;
    public Quaternion rotOffset;

    public float attackSpeed;
    public float range;



    public virtual void Start()
    {
        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
        towerPreviewRenderer.transform.localScale = Vector3.one * range;
    }
    public virtual void Init()
    {
        DissolveController[] dissolves = GetComponentsInChildren<DissolveController>();
        amountOfDissolves = dissolves.Length;

        foreach (var dissolve in dissolves)
        {
            dissolve.Init(this);
        }

        towerPreviewRenderer.enabled = false;
    }

    public virtual void SelectOrDeselectTower(bool select)
    {
        DissolveController[] dissolves = GetComponentsInChildren<DissolveController>();

        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetInt("_Selected", select ? 1 : 0);
        }
    }

    public void DissolveCompleted()
    {
        cDissolves += 1;
        if (cDissolves == amountOfDissolves)
        {
            towerCompleted = true;
        }
    }
}