using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AIO_AreaEffect : MonoBehaviour
{
    public SphereCollider sphereColl;
    public BoxCollider boxColl;

    private ProjectileStats p;
    private int splashHitsLeft;

    public void Init(ProjectileStats _p)
    {
        if (sphereColl != null)
        {
            sphereColl.enabled = true;
            sphereColl.radius = p.AIO_areaEffectSize.x / 2;
        }
        else
        {
            boxColl.enabled = true;
            boxColl.size = p.AIO_areaEffectSize;
        }



        p = _p;

        splashHitsLeft = p.AIO_maxSplashHits;
        if (splashHitsLeft == 0)
        {
            splashHitsLeft = -1;
        }

        if (p.AIO_duration != 0)
        {
            Destroy(gameObject, p.AIO_duration);
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        print(splashHitsLeft);
        if (splashHitsLeft != 0 && obj.TryGetComponent(out EnemyCore target))
        {
            splashHitsLeft -= 1;
            OnHitTarget(target);
        }
    }

    public virtual void OnHitTarget(EnemyCore target)
    {
        print(target.health);
        target.ApplyDamage(p.AIO_damageType, p.AIO_damage, p.AIO_damageOverTimeType, p.AIO_damageOverTime, p.AIO_time, p.confusionTime, p.slownessPercentage, p.slownessTime);
        print(target.health);
    }
}
