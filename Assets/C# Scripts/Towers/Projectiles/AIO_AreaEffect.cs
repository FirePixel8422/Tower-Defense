using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIO_AreaEffect : MonoBehaviour
{
    private SphereCollider coll;
    private ProjectileStats projStats;

    public void Init(ProjectileStats _projStats)
    {
        coll = GetComponent<SphereCollider>();
        projStats = _projStats;
        coll.enabled = true;
        coll.radius = projStats.areaEffectSize;
        Destroy(gameObject, projStats.duration);
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.TryGetComponent(out EnemyCore e))
        {
            e.ApplyDamage(projStats.AIO_damageType, projStats.AIO_damage, projStats.AIO_damageOverTimeType, projStats.AIO_damageOverTime, projStats.AIO_time);
        }
    }
}
