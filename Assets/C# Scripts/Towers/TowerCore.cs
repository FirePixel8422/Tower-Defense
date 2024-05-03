using System.Collections;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    public Material dissolveMaterial;

    private float cDissolveEffectState;
    public float startDissolveEffectState;
    public float dissolveSpeed;
    public float endDisolveValue;

    public SpriteRenderer towerPreviewRenderer;



    private void Start()
    {
        dissolveMaterial = GetComponent<Renderer>().material;
        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public void Init()
    {
        StartCoroutine(CreateTower());
    }

    private IEnumerator CreateTower()
    {
        towerPreviewRenderer.enabled = false;

        cDissolveEffectState = startDissolveEffectState;
        while (cDissolveEffectState > endDisolveValue)
        {
            yield return null;
            cDissolveEffectState -= Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_Disolve_Active", cDissolveEffectState);
        }
    }
}