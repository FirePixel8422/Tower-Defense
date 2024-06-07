using System.Collections;
using UnityEngine;

public class AIO_AreaEffect : MonoBehaviour
{
    public int areaEffectId;

    public SphereCollider sphereColl;
    public BoxCollider boxColl;

    public ProjectileStats p;
    private int splashHitsLeft;

    public void Init(ProjectileStats _p)
    {
        p = _p;

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


        splashHitsLeft = p.AIO_maxSplashHits;
        if (splashHitsLeft == 0)
        {
            splashHitsLeft = -1;
        }

        if (p.AIO_duration != 0)
        {
            StartCoroutine(DisableDelay());
        }
    }

    private IEnumerator DisableDelay()
    {
        yield return new WaitForSeconds(p.AIO_duration);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (splashHitsLeft != 0 && obj.TryGetComponent(out EnemyCore target))
        {
            splashHitsLeft -= 1;
            OnHitTarget(target);

            if(splashHitsLeft == 0)
            {
                gameObject.SetActive(false);
                sphereColl.enabled = false;
                boxColl.enabled = false;
            }
        }
    }

    public virtual void OnHitTarget(EnemyCore target)
    {
        target.ApplyDamage(p.AIO_damageType, p.AIO_damage, p.AIO_damageOverTimeType, p.AIO_damageOverTime, p.AIO_time, p.confusionTime, p.slownessPercentage, p.slownessTime);
    }
}
