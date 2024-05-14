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

    public GameObject projectile;
    public Transform shootPoint;

    public EnemyCore target;

    public MagicType[] magicType;
    public float rotSpeed;
    public Transform rotPoint;
    public Quaternion rotOffset;

    public float attackSpeed;
    public float speed;
    public float damage;
    public float range;
    public float lookTreshold;



    public virtual void Start()
    {
        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
        towerPreviewRenderer.transform.localScale = Vector3.one * range;
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
        while (true)
        {
            yield return new WaitForSeconds(attackSpeed);
            yield return new WaitUntil(() => target != null && Vector3.Dot(shootPoint.forward, (target.transform.position - transform.position).normalized) >= lookTreshold);
            Shoot();
        }
    }
    public virtual void Shoot()
    {
        MagicProjectile bullet = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<MagicProjectile>();
        bullet.Init(target, speed, damage);
        target.TryHit(damage);
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