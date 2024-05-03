using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyCore : MonoBehaviour
{
    public float health;
    public float damage;
    public float mapCompletionTime;
    public MagicType[] elements;

    public MagicType immunityBarrier;
    public Material barrierShader;

    [HideInInspector]
    public SplineAnimate splineAnimator;


    public void Init(SplineContainer spline, ImmunityBarrier _immunityBarrier)
    {
        splineAnimator = GetComponent<SplineAnimate>();
        splineAnimator.Container = spline;
        splineAnimator.Duration = mapCompletionTime;

        if (_immunityBarrier != ImmunityBarrier.None)
        {
            if (_immunityBarrier == ImmunityBarrier.Smart)
            {
                immunityBarrier = TowerManager.Instance.HighestMagicType();
            }
            barrierShader.GameObject().SetActive(true);
        }

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
