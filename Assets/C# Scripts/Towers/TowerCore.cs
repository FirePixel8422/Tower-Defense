using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class TowerCore : MonoBehaviour
{
    public DissolveController[] dissolves;

    [HideInInspector]
    public int amountOfDissolves;
    [HideInInspector]
    public int cDissolves;

    public float towerCost;
    public MagicType costType;

    public TowerUIDataSO towerUIData;
    public GameObject[] upgradePrefabs;

    [HideInInspector]
    public bool towerCompleted;

    public bool placeOntrack;

    [HideInInspector]
    public SpriteRenderer towerPreviewRenderer;
    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public AudioController audioController;

    public Projectile projectile;
    public Transform shootPoint;

    //[HideInInspector]
    public EnemyCore target;
    public TargetMode targetMode;
    public bool specialTargetMode;

    public TextMeshProUGUI targetModeTextObj;

    private int targetId;
    public string targetModeString;


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
        dissolves = GetComponentsInChildren<DissolveController>();

        towerPreviewRenderer = GetComponentInChildren<SpriteRenderer>();
        towerPreviewRenderer.transform.localScale = Vector3.one * range;
        anim = GetComponent<Animator>();
        audioController = GetComponent<AudioController>();
    }
    public virtual void CoreInit()
    {
        amountOfDissolves = dissolves.Length;
        foreach (var dissolve in dissolves)
        {
            dissolve.StartDissolve(this);
        }

        UpdatePreviewTower(false);

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

        target.TryHit(projStats.damageType, projStats.damage, projStats.doSplashDamage, projStats.AIO_damageType, projStats.AIO_damage);

        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        Projectile bullet = ProjectilePooling.Instance.GetPulledObj(projectile.projectileId, shootPoint.position, Quaternion.Euler(0, angle, 0)).GetComponent<Projectile>();
        bullet.Init(target, projStats);
    }

    public virtual void SelectOrDeselectTower(bool select)
    {
        towerPreviewRenderer.enabled = select;

        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetInt("_Selected", select ? 1 : 0);
        }
    }

    public string NextTargetMode()
    {
        targetId += 1;
        if (targetId == 4)
        {
            targetId = 0;
        }
        targetModeString = UpdateTargetMode(targetId);
        return specialTargetMode ? "Special" : targetModeString;
    }
    public string PreviousTargetMode()
    {
        targetId -= 1;
        if (targetId == -1)
        {
            targetId = 3;
        }
        targetModeString = UpdateTargetMode(targetId);
        return specialTargetMode ? "Special" : targetModeString;
    }
    private string UpdateTargetMode(int targetId)
    {
        if (targetId == 3)
        {
            targetMode = TargetMode.Tanky;
            return "Tanky";
        }
        else if (targetId == 2)
        {
            targetMode = TargetMode.Dangerous;
            return "Dangerous";
        }
        else if (targetId == 1)
        {
            targetMode = TargetMode.Last;
            return "Last";
        }
        else
        {
            targetMode = TargetMode.First;
            return "First";
        }
    }


    public virtual void UpdatePreviewTower(bool preview)
    {
        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetInt("_Preview", preview ? 1 : 0);
        }
    }

    public void DisplayColorOfTowerPreview(Color color)
    {
        foreach (var d in dissolves)
        {
            d.dissolveMaterial.SetColor("_PreviewColor", color);
        }
    }



    public void UpgradeTower(bool leftPath)
    {
        //spawn upgrade
        TowerCore tower = Instantiate(upgradePrefabs[leftPath ? 0 : 1], transform.position, upgradePrefabs[leftPath ? 0 : 1].transform.rotation).GetComponent<TowerCore>();

        //dissolve old (this) tower away
        DissolveController[] dissolves = GetComponentsInChildren<DissolveController>();
        foreach (var dissolve in dissolves)
        {
            dissolve.Revert(this);
        }
        tower.CoreInit();
    }

    public void DissolveCompleted()
    {
        cDissolves += 1;
        if (cDissolves == amountOfDissolves)
        {
            towerCompleted = true;
        }
    }
    public void RevertCompleted()
    {
        cDissolves -= 1;
        if (cDissolves == 0)
        {
            Destroy(gameObject);
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