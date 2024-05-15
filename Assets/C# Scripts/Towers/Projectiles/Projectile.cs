using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
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
        //loop and move bullet towards target. then check distance between this projectile and target and check if they collided (Vector3.Distance)
        while (true)
        {
            yield return null;
            if (target == null)
            {
                //tracked target died, destroy bullet
                Destroy(gameObject);
                print("killed self");
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, projStats.speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.transform.position) < size)
            {
                //call virtual void "HitTarget()"
                HitTarget();
                yield break;
            }
        }
    }
    public virtual void HitTarget()
    {
        if (projStats.doSplashDamage)
        {
            //spawn collider to hit multiple bullets
            ApplySplashDamage();
        }
        else
        {
            target.ApplyDamage(projStats.damageType, projStats.damage, projStats.damageOverTimeType, projStats.damageOverTime, projStats.time);
        }

        //damage and effects applies, destroy bullet
        Destroy(gameObject);
    }
    public virtual void ApplySplashDamage()
    {

    }
}




[System.Serializable]
public class ProjectileStats
{
    public float speed;
    public MagicType damageType;
    public float damage;
    public MagicType damageOverTimeType;
    public float damageOverTime;
    public float time;


    public bool doSplashDamage;
    public int maxSplashHits;
    public GameObject splashObject;
    public float splashDuration;
}