using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTrap : TowerCore
{
    public bool smart;
    public float trapActivateTime;
    public float trapDeActivateTime;

    public int enemiesOnTrap;
    public BoxCollider trapColl;

    private float timer = 0;


    public override void Init()
    {
        trapColl = GetComponent<BoxCollider>();
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            timer -= Time.deltaTime;
            yield return null;

            if (timer < 0 && (enemiesOnTrap > 0 || smart == false))
            {
                timer = attackSpeed;
                StartCoroutine(Attack());
            }
        }
    }

    public IEnumerator Attack()
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
        if (obj.transform.TryGetComponent(out EnemyCore e))
        {
            e.ApplyDamage(projStats.damageType, projStats.damage,projStats.damageOverTimeType, projStats.damageOverTime, projStats.time);
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
