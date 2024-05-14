using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagicProjectile : MonoBehaviour
{
    public float size;

    public EnemyCore target;
    public ProjectileStats projStats;



    public void Init(EnemyCore _target, ProjectileStats _s)
    {
        target = _target;
        projStats = _s;

        StartCoroutine(TrackTargetLoop());
    }


    private IEnumerator TrackTargetLoop()
    {
        while (true)
        {
            yield return null;
            if (target == null)
            {
                Destroy(gameObject);
                yield break;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, projStats.speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.transform.position) < size)
            {
                target.ApplyDamage(projStats.damage, projStats.damageOverTime, projStats.time);

                Destroy(gameObject);
                yield break;
            }
        }
    }

}


[System.Serializable]
public class ProjectileStats
{
    public float speed;
    public float damage;
    public float damageOverTime;
    public float time;


    public bool splashDamage;
    public int maxSplashHits;
    public GameObject splashObject;
    public float splashDuration;
}