using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
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
            if (Vector3.Distance(transform.position, target.transform.position) < projStats.projectileSize)
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
        AIO_AreaEffect AIO_Effect = Instantiate(projStats.areaEffect, target.transform.position, Quaternion.identity);
        AIO_Effect.Init(projStats);
    }
}




[System.Serializable]
public struct ProjectileStats
{
    public float projectileSize;

    public float speed;
    public MagicType damageType;
    public float damage;
    public MagicType damageOverTimeType;
    public float damageOverTime;
    public float time;

    public AIO_AreaEffect areaEffect;
    public float duration;

    public bool doSplashDamage;
    public float areaEffectSize;
    public int maxSplashHits;

    public MagicType AIO_damageType;
    public float AIO_damage;
    public MagicType AIO_damageOverTimeType;
    public float AIO_damageOverTime;
    public float AIO_time;
}