using System.Collections;
using UnityEngine;

public class ImmunityBarrier : MonoBehaviour
{
    private float maxBarrierHealth;
    public float barrierHealth;

    public Material barrierShader;

    [ColorUsage(true, true)]
    public Color[] frontColors;

    [ColorUsage(true, true)]
    public Color[] backColors;

    [ColorUsage(true, true)]
    public Color[] fresnelColors;


    public void Init(float _barrierHealth, float healthDrainTime, int immunityBarrierIndex)
    {
        maxBarrierHealth = _barrierHealth;
        barrierHealth = _barrierHealth;
        transform.localScale = Vector3.one * 2;

        barrierShader = GetComponent<Renderer>().material;
        barrierShader.SetColor("_FrontColor", frontColors[immunityBarrierIndex]);
        barrierShader.SetColor("_BackColor", backColors[immunityBarrierIndex]);
        barrierShader.SetColor("_FresnelColor", fresnelColors[immunityBarrierIndex]);

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
