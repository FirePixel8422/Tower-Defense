using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AIO_AreaEffect : MonoBehaviour
{
    public SphereCollider sphereColl;
    public BoxCollider boxColl;

    private ProjectileStats projStats;
    private int splashHitsLeft;

    public void Init(ProjectileStats _projStats)
    {
        if (sphereColl != null)
        {
            sphereColl.enabled = true;
            sphereColl.radius = projStats.AIO_areaEffectSize.x / 2;
        }
        else
        {
            boxColl.enabled = true;
            boxColl.size = projStats.AIO_areaEffectSize;
        }



        projStats = _projStats;

        splashHitsLeft = projStats.AIO_maxSplashHits;
        if (splashHitsLeft == 0)
        {
            splashHitsLeft = -1;
        }

        if (projStats.AIO_duration != 0)
        {
            Destroy(gameObject, projStats.AIO_duration);
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (splashHitsLeft != 0 && obj.TryGetComponent(out EnemyCore target))
        {
            splashHitsLeft -= 1;
            OnHitTarget(target);
        }
    }

    public virtual void OnHitTarget(EnemyCore target)
    {
        target.ApplyDamage(projStats.AIO_damageType, projStats.AIO_damage, projStats.AIO_damageOverTimeType, projStats.AIO_damageOverTime, projStats.AIO_time, projStats.confusionTime);
    }
}
