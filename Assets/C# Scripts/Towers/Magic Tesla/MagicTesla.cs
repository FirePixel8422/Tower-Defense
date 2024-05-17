using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.GraphicsBuffer;

public class MagicTesla : TowerCore
{
    private VisualEffect effect;

    public float timePerCharge;
    public float rechargePowerDelay;
    private float timeSinceLastAttack;

    private int maxCharges;
    public int charges;
    public int amountOfTargets;

    public float damageDelay;

    public override void Init()
    {
        maxCharges = charges;
        effect = GetComponentInChildren<VisualEffect>();
        StartCoroutine(RechargePower());
        StartCoroutine(ShootLoop());
    }

    private IEnumerator RechargePower()
    {
        float timer = 0;
        while (true)
        {
            timer -= Time.deltaTime;

            yield return null;
            yield return new WaitUntil(() => charges != maxCharges);

            while (timeSinceLastAttack < rechargePowerDelay)
            {
                timeSinceLastAttack += Time.deltaTime;
                yield return null;
            }
            if (timer < 0)
            {
                charges += 1;
                timer = timePerCharge;
            }
        }
    }

    private IEnumerator ShootLoop()
    {
        float timer = 0;
        while (true)
        {
            timer -= Time.deltaTime;
            yield return null;

            if (timer < 0)
            {
                if (target != null &&
                    charges != 0 &&
                    target.incomingDamage < target.health &&
                    Vector3.Dot(shootPoint.forward, (target.transform.position - transform.position).normalized) >= lookTreshold)
                {
                    Shoot();
                    charges -= 1;
                    timeSinceLastAttack = 0;
                    timer = attackSpeed;
                }
            }
        }
    }

    public override void Shoot()
    {
        List<EnemyCore> targets = new List<EnemyCore>
        {
            target
        };

        for (int i = 0; i < amountOfTargets - 1; i++)
        {
            float progression = -1;
            int[] ids = new int[amountOfTargets - 1];
            Array.Fill(ids, -1);


            for (int i2 = 0; i2 < WaveManager.Instance.spawnedObj.Count; i2++)
            {
                if (targets.Contains(WaveManager.Instance.spawnedObj[i2]))
                {
                    continue;
                }

                if (Vector3.Distance(transform.position, WaveManager.Instance.spawnedObj[i2].transform.position) < range)
                {
                    if (WaveManager.Instance.spawnedObj[i2].incomingDamage < WaveManager.Instance.spawnedObj[i2].health)
                    {
                        if (WaveManager.Instance.spawnedObj[i2].progression > progression)
                        {
                            progression = WaveManager.Instance.spawnedObj[i2].progression;
                            ids[i] = i2;
                        }
                    }
                }
            }
            if (ids[i] != -1)
            {
                targets.Add(WaveManager.Instance.spawnedObj[ids[i]]);
                targets[targets.Count - 1].TryHit(projStats.damageType, projStats.damage);
            }
        }

        StartCoroutine(DamageImpactDelay(targets));
    }

    private IEnumerator DamageImpactDelay(List<EnemyCore> targets)
    {
        foreach (EnemyCore target in targets)
        {
            effect.SetVector3(Shader.PropertyToID("Pos"), target.transform.position);
            effect.Play();
            yield return null;
        }

        yield return new WaitForSeconds(damageDelay);
        foreach (EnemyCore target in targets)
        {
            target.ApplyDamage(projStats.damageType, projStats.damage, projStats.damageOverTimeType, projStats.damageOverTime, projStats.time);
        }
    }
}
