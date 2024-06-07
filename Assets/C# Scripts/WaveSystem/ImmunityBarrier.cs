using System.Collections;
using UnityEngine;

public class ImmunityBarrier : MonoBehaviour
{
    private float maxBarrierHealth;
    public float barrierHealth;

    public Vector3 startScale;
    public Vector3 killScale;
    private Vector3 scaleIncrement;

    public Material barrierShader;

    [ColorUsage(true, true)]
    public Color[] frontColors;

    [ColorUsage(true, true)]
    public Color[] backColors;

    [ColorUsage(true, true)]
    public Color[] fresnelColors;

    private void Start()
    {
        scaleIncrement = startScale - killScale;
    }


    public void Init(float _barrierHealth, float healthDrainTime, int immunityBarrierIndex)
    {
        maxBarrierHealth = _barrierHealth;
        barrierHealth = _barrierHealth;
        transform.localScale = startScale;

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
            transform.localScale = startScale - scaleIncrement * (1 - (barrierHealth / maxBarrierHealth));
        }
        if (barrierHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        barrierHealth -= damage;
        transform.localScale = startScale - Vector3.one * (1 - (barrierHealth / maxBarrierHealth));
        if (barrierHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
