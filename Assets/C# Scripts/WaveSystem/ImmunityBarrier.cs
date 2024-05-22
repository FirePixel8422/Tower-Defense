using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class ImmunityBarrier : MonoBehaviour
{
    private float maxBarrierHealth;
    public float barrierHealth;

    public Material barrierShader;

    [ColorUsage(true, true)]
    public Color frontColor;

    [ColorUsage(true, true)]
    public Color backColor;

    [ColorUsage(true, true)]
    public Color fresnelColor;


    public void Init(float _barrierHealth, float healthDrainTime)
    {
        maxBarrierHealth = _barrierHealth;
        barrierHealth = _barrierHealth;
        transform.localScale = Vector3.one * 2;

        barrierShader = GetComponent<Renderer>().material;
        barrierShader.SetColor("_FrontColor", frontColor);
        barrierShader.SetColor("_BackColor", backColor);
        barrierShader.SetColor("_FresnelColor", fresnelColor);

        if(healthDrainTime > 0)
        {
            StartCoroutine(HealthDrainTimer(healthDrainTime));
        }
    }
    private IEnumerator HealthDrainTimer(float healthDrainTime)
    {
        while (barrierHealth > 0)
        {
            yield return null;
            barrierHealth -= maxBarrierHealth / healthDrainTime * Time.deltaTime;
            barrierShader.SetFloat("_KillValue", -(1 - barrierHealth / maxBarrierHealth) * 4 + 1);
            transform.localScale = Vector3.one * 2 - Vector3.one * (1 - (barrierHealth / maxBarrierHealth));
        }
        if (barrierHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        barrierShader.SetFloat("_KillValue", -(1 - barrierHealth / maxBarrierHealth) * 4 + 1);
        barrierHealth -= damage;
        transform.localScale = Vector3.one * 2 - Vector3.one * (1 - (barrierHealth / maxBarrierHealth));
        if (barrierHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
