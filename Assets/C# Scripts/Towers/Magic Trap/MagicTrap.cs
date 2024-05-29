using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MagicTrap : TowerCore
{
    public bool smart;
    public float trapActivateTime;
    public float trapDeActivateTime;

    public float confusionTime;

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
        yield return new WaitForSeconds(trapDeActivateTime);
        trapColl.enabled = false;
    }


    private void OnCollisionEnter(Collision obj)
    {
        if (obj.transform.TryGetComponent(out EnemyCore target))
        {
            target.ApplyDamage(projStats.damageType, projStats.damage, projStats.damageOverTimeType, projStats.damageOverTime, projStats.time);
            if (confusionTime > 0)
            {
                StartCoroutine(ConfuseEnemyTimer(target));
            }
        }
    }

    private IEnumerator ConfuseEnemyTimer(EnemyCore target)
    {
        target.moveSpeed = 0;
        while (true)
        {
            yield return new WaitForSeconds(confusionTime);

            target.moveSpeed = target.startMoveSpeed;

            yield break;
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
