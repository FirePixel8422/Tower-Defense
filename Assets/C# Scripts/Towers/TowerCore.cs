using System.Collections;
using UnityEngine;


public class TowerCore : MonoBehaviour
{
    [HideInInspector]
    public DissolveController[] dissolves;
    [HideInInspector]
    public int amountOfDissolves;
    [HideInInspector]
    public int cDissolves;

    public TowerUIDataSO towerUIData;
    public GameObject[] upgradePrefabs;

    [HideInInspector]
    public bool towerCompleted;

    [HideInInspector]
    public SpriteRenderer towerPreviewRenderer;
    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public AudioController audioController;

    public Projectile projectile;
    public int onHitEffectIndex = -1;
    public Transform shootPoint;

    //[HideInInspector]
    public EnemyCore target;
    public TargetMode targetMode;
    [HideInInspector]
    public bool excludeTargetUpdates;
    [HideInInspector] 
    public bool specialTargetMode;

    [HideInInspector]
    public int targetId;
    [HideInInspector]
    public string targetModeString;


    public MagicType magicType;
    public float rotSpeed;
    public Transform rotPoint;
    public Quaternion rotOffset;

    public float range;
    public float lookTreshold;
    public float attackSpeed;

    public ProjectileStats p;


    public virtual void Start()
    {
        if (magicType != MagicType.Neutral)
        {
            TowerManager.Instance.magicValues[(int)magicType - 1] += 1;
        }
        SetupTower();
    }

    private void SetupTower()
    {
        dissolves = GetComponentsInChildren<DissolveController>();
        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
        towerPreviewRenderer.transform.localScale = Vector3.one * range;
        anim = GetComponent<Animator>();
        audioController = GetComponent<AudioController>();
    }
    public virtual void CoreInit()
    {
        SetupTower();

        amountOfDissolves = dissolves.Length;
        foreach (var dissolve in dissolves)
        {
            dissolve.StartDissolve(this);
        }

        towerPreviewRenderer.enabled = false;
        Init();
    }
    public virtual void Init()
    {
        StartCoroutine(ShootLoop());
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
                    target.IsNotAboutToDie &&
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
        audioController.Play();

        target.TryHit(p.damageType, p.damage, p.AIO_damageType, p.AIO_damage);

        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        Projectile bullet = ProjectilePooling.Instance.GetPulledObj(projectile.projectileId, shootPoint.position, Quaternion.Euler(0, angle, 0)).GetComponent<Projectile>();
        bullet.Init(target, p, onHitEffectIndex);
    }

    public virtual void SelectOrDeselectTower(bool select)
    {
        towerPreviewRenderer.enabled = select;

        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetInt("_Selected", select ? 1 : 0);
        }
    }

    public virtual string UpdateTargetMode(int direction)
    {
        targetId += direction;
        if (targetId == 4)
        {
            targetId = 0;
        }
        if (targetId == -1)
        {
            targetId = 3;
        }

        string targetModeString;
        if (specialTargetMode)
        {
            return "Special";
        }
        if (targetId == 3)
        {
            targetMode = TargetMode.Tanky;
            targetModeString = "Tanky";
        }
        else if (targetId == 2)
        {
            targetMode = TargetMode.Dangerous;
            targetModeString = "Dangerous";
        }
        else if (targetId == 1)
        {
            targetMode = TargetMode.Last;
            targetModeString = "Last";
        }
        else
        {
            targetMode = TargetMode.First;
            targetModeString = "First";
        }

        return targetModeString;
    }


    public void UpdateTowerPreviewColor(Color color)
    {
        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetColor("_PreviewColor", color);
        }
    }



    public void UpgradeTower(int path, Vector2Int gridPos)
    {
        StopAllCoroutines();

        //dissolve old (this) tower away
        DissolveController[] dissolves = GetComponentsInChildren<DissolveController>();
        foreach (var dissolve in dissolves)
        {
            dissolve.Revert(this);
        }

        //spawn upgrade
        TowerCore tower = Instantiate(upgradePrefabs[path], transform.position, upgradePrefabs[path].transform.rotation).GetComponent<TowerCore>();

        if (excludeTargetUpdates == false)
        {
            TowerManager.Instance.spawnedTowerObj.Add(tower);
            AudioManager.Instance.audioControllers.Add(tower.audioController);
        }
        tower.CoreInit();
        GridManager.Instance.UpdateGridDataFieldType(gridPos, 3, tower);
    }

    public void DissolveCompleted()
    {
        cDissolves += 1;
        if (cDissolves == amountOfDissolves)
        {
            TowerCompleted();
        }
    }
    public virtual void TowerCompleted()
    {
        towerCompleted = true;
    }
    public void RevertCompleted()
    {
        cDissolves -= 1;
        if (cDissolves == 0)
        {
            Destroy(gameObject);
            AudioManager.Instance.audioControllers.Remove(audioController);
        }
    }

    private void OnDestroy()
    {
        if (magicType != MagicType.Neutral)
        {
            TowerManager.Instance.magicValues[(int)magicType - 1] -= 1;
        }
    }
}

public enum TargetMode
{
    First,
    Last,
    Dangerous,
    Tanky
};