using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicTesla : TowerCore
{
    private ParticleSystem pSystem;
    public float maxCoilEmisionRate;
    private VisualEffect effect;

    public float timePerCharge;
    public float rechargePowerDelay;
    private float timeSinceLastAttack;

    public int maxCharges;
    private int charges;
    public int amountOfTargets;

    public float damageDelay;

    private int propertyId;

    public override void Init()
    {
        pSystem = GetComponentInChildren<ParticleSystem>();
        effect = GetComponentInChildren<VisualEffect>();

        propertyId = Shader.PropertyToID("Pos");

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
            yield return new WaitUntil(() => charges < maxCharges);

            while (timeSinceLastAttack < rechargePowerDelay)
            {
                timeSinceLastAttack += Time.deltaTime;
                yield return null;
            }
            if (timer < 0)
            {
                charges += 1;                
                timer = timePerCharge;

#pragma warning disable CS0618 // Type or member is obsolete
                pSystem.emissionRate = maxCoilEmisionRate / maxCharges * charges;
#pragma warning restore CS0618 // Type or member is obsolete
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
                    target.incomingDamage < target.health)
                {
                    Shoot();
                    charges -= 1;

                    #pragma warning disable CS0618 // Type or member is obsolete
                    pSystem.emissionRate = Mathf.Clamp(maxCoilEmisionRate / maxCharges * charges, 1.5f, float.MaxValue);
                    #pragma warning restore CS0618 // Type or member is obsolete

                    timeSinceLastAttack = 0;
                    timer = attackSpeed;
                }
            }
        }
    }

    public override void Shoot()
    {
        audioController.Play();
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
                targets[^1].TryHit(projStats.damageType, projStats.damage, projStats.doSplashDamage, projStats.AIO_damageType, projStats.AIO_damage);
            }
        }

        StartCoroutine(DamageImpactDelay(targets));
    }

    private IEnumerator DamageImpactDelay(List<EnemyCore> targets)
    {
        foreach (EnemyCore target in targets)
        {
            effect.SetVector3(propertyId, target.transform.position);
            effect.Play();
            yield return null;
        }

        yield return new WaitForSeconds(damageDelay);
        foreach (EnemyCore target in targets)
        {
            if (target.health > 0)
            {
                target.ApplyDamage(projStats.damageType, projStats.damage, projStats.damageOverTimeType, projStats.damageOverTime, projStats.time);
            }
        }
    }
}
