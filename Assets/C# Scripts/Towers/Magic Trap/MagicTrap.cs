using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTrap : TowerCore
{
    public bool smart;
    public float trapActivateTime;
    public float trapDeActivateTime;

    private BoxCollider trapColl;

    private float timer = 0;


    public override void Init()
    {
        specialTargetMode = true;
        excludeTargetUpdates = true;
        trapColl = GetComponent<BoxCollider>();
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitUntil(() => towerCompleted);
        while (true)
        {
            timer -= Time.deltaTime;
            yield return null;

            if (timer < 0 && smart == false)
            {
                timer = attackSpeed;
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        anim.SetTrigger("Attack");
        audioController.Play();

        yield return new WaitForSeconds(trapActivateTime);

        trapColl.enabled = true;
        if (p.areaEffect != null)
        {
            ApplySplashDamage();
        }

        yield return new WaitForSeconds(trapDeActivateTime);

        trapColl.enabled = false;
    }

    private void ApplySplashDamage()
    {
        AIO_AreaEffectPooling.Instance.GetPulledObj(p.areaEffect.areaEffectId, transform.position, Quaternion.identity, p);
    }


    private void OnCollisionEnter(Collision obj)
    {
        if (obj.transform.TryGetComponent(out EnemyCore target))
        {
            target.ApplyDamage(p.damageType, p.damage, p.damageOverTimeType, p.damageOverTime, p.time, p.confusionTime, p.slownessPercentage, p.slownessTime, p.maxSlowStacks);
        }
    }

    private void OnTriggerStay(Collider obj)
    {
        if (smart == false)
        {
            return;
        }
        if (obj.gameObject.CompareTag("Enemy"))
        {
            if (timer < 0)
            {
                StartCoroutine(Attack());
                timer = attackSpeed;
            }
        }
    }
}
