using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    public int amountOfDissolves;
    public int cDissolves;

    public bool towerCompleted;

    [HideInInspector]
    public SpriteRenderer towerPreviewRenderer;
    [HideInInspector]
    public Animator anim;

    public GameObject projectile;
    public Transform shootPoint;

    public EnemyCore target;

    public MagicType[] magicType;
    public float rotSpeed;
    public Transform rotPoint;
    public Quaternion rotOffset;

    public float range;
    public float lookTreshold;
    public float attackSpeed;

    public ProjectileStats projStats;


    public virtual void Start()
    {
        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
        towerPreviewRenderer.transform.localScale = Vector3.one * range;
        anim = GetComponent<Animator>();
    }
    public virtual void Init()
    {
        StartCoroutine(ShootLoop());
        DissolveController[] dissolves = GetComponentsInChildren<DissolveController>();
        amountOfDissolves = dissolves.Length;

        foreach (var dissolve in dissolves)
        {
            dissolve.Init(this);
        }

        towerPreviewRenderer.enabled = false;
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
                    target.incomingDamage < target.health &&
                    Vector3.Dot(shootPoint.forward, (target.transform.position - transform.position).normalized) >= lookTreshold)
                {
                    Shoot();
                    timer = attackSpeed;
                }
            }
        }
    }

    public virtual void Shoot()
    {
        anim.SetTrigger("Shoot");
        target.TryHit(projStats.damage);

        MagicProjectile bullet = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<MagicProjectile>();
        bullet.Init(target, projStats);
    }

    public virtual void SelectOrDeselectTower(bool select)
    {
        DissolveController[] dissolves = GetComponentsInChildren<DissolveController>();

        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetInt("_Selected", select ? 1 : 0);
        }
    }

    public void DissolveCompleted()
    {
        cDissolves += 1;
        if (cDissolves == amountOfDissolves)
        {
            towerCompleted = true;
        }
    }
}