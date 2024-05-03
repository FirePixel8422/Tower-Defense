using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    #region disolveEffect
    public Material dissolveMaterial;

    private float cDissolveEffectState;
    public float startDissolveEffectState;
    public float dissolveSpeed;
    public float endDisolveValue;
    #endregion


    public SpriteRenderer towerRange;


    public Element[] elements;
    public float attackSpeed;
    public float range;



    private void Start()
    {
        dissolveMaterial = GetComponent<Renderer>().material;

        towerRange = GetComponentInChildren<SpriteRenderer>();
        towerRange.transform.localScale = Vector3.one * range;
    }
    public void Init()
    {
        StartCoroutine(CreateTower());
    }
    private IEnumerator CreateTower()
    {
        towerRange.enabled = false;

        cDissolveEffectState = startDissolveEffectState;
        while (cDissolveEffectState > endDisolveValue)
        {
            yield return null;
            cDissolveEffectState -= Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_Disolve_Active", cDissolveEffectState);
        }
    }
}