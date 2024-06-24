using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicExplosive : TowerCore
{
    public Transform explosionPoint;

    public Collider explosionTrigger;
    public Collider explosionColl;

    public float explosionDelay;
    public float explosionTime;

    public override void Init()
    {
        specialTargetMode = true;
        excludeTargetUpdates = true;
    }

    public override void TowerCompleted()
    {
        base.TowerCompleted();
        explosionColl.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            anim.SetTrigger("Boom");
            StartCoroutine(ExplodeDelay());
        }
    }
    private void OnTriggerEnter(Collider obj)
    {
        if (obj.transform.TryGetComponent(out EnemyCore target))
        {
            target.ApplyDamage(p.damageType, p.damage, p.damageOverTimeType, p.damageOverTime, p.time, p.confusionTime, p.slownessPercentage, p.slownessTime, p.maxSlowStacks);
        }
    }

    private IEnumerator ExplodeDelay()
    {
        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(explosionDelay);
        explosionTrigger.enabled = true;

        OnHitVFXPooling.Instance.GetPulledObj(onHitEffectIndex, explosionPoint.position, Quaternion.identity);

        GridObjectData gridData = GridManager.Instance.GridObjectFromWorldPoint(transform.position);
        GridManager.Instance.ResetGridDataFieldType(gridData.gridPos);

        Destroy(gameObject, explosionTime);
    }
}
