using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyCore : MonoBehaviour
{
    public float health;
    public float damage;
    public float mapCompletionTime;
    public Element[] elements;

    [HideInInspector]
    public SplineAnimate splineAnimator;


    public void Init(SplineContainer spline)
    {
        splineAnimator = GetComponent<SplineAnimate>();
        splineAnimator.Container = spline;
        splineAnimator.Duration = mapCompletionTime;
        StartCoroutine(StartDelay());
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.05f);
        splineAnimator.Play();
    }


    public void OnTriggerEnter(Collider obj)
    {
        if (obj.TryGetComponent(out TerrainEffect terrainInfo))
        {

        }
    }
}
