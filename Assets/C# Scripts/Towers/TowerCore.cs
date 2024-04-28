using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    public Material dissolveMaterial;
    public float dissolveEffectState;
    public float dissolveSpeed;
    public float endDisolveValue;



    private void Start()
    {
        dissolveMaterial = GetComponent<Renderer>().material;
    }

    private IEnumerator CreateTower()
    {
        while (dissolveEffectState > endDisolveValue)
        {
            yield return null;
            dissolveEffectState -= Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_Active", dissolveEffectState);
        }
    }
}