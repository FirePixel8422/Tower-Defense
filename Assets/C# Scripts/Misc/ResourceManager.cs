using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    private void Awake()
    {
        Instance = this;
    }



    public float scrap;
    public float Scrap
    {
        get
        {
            return scrap;
        }
        set
        {
            scrap = value;
            float rounded = Mathf.Round(scrap);
            if (Mathf.Approximately(scrap, rounded))
            {
                scrap = rounded;
            }
            OnResourceChanged.Invoke();
        }
    }

    public float lifeEssence;
    public float LifeEssence
    {
        get
        {
            return lifeEssence;
        }
        set
        {
            lifeEssence = value;
            float rounded = Mathf.Round(lifeEssence);
            if (Mathf.Approximately(lifeEssence, rounded))
            {
                lifeEssence = rounded;
            }
            OnResourceChanged.Invoke();
        }
    }
    public float arcaneEssence;
    public float ArcaneEssence
    {
        get
        {
            return arcaneEssence;
        }
        set
        {
            arcaneEssence = value;
            float rounded = Mathf.Round(arcaneEssence);
            if (Mathf.Approximately(arcaneEssence, rounded))
            {
                arcaneEssence = rounded;
            }
            OnResourceChanged.Invoke();
        }
    }
    public float emberEssence;
    public float EmberEssence
    {
        get
        {
            return emberEssence;
        }
        set
        {
            emberEssence = value;
            float rounded = Mathf.Round(emberEssence);
            if (Mathf.Approximately(emberEssence, rounded))
            {
                emberEssence = rounded;
            }
            OnResourceChanged.Invoke();
        }
    }


    public UnityEvent OnResourceChanged;

    public TextMeshProUGUI scrapTextObj;

    public TextMeshProUGUI lifeEssenceTextObj;
    public TextMeshProUGUI arcaneEssenceTextObj;
    public TextMeshProUGUI emberEssenceTextObj;

    private const float zeroPointThree = 1 / 3;


    private void Start()
    {
        OnResourceChanged.AddListener(UpdateResourceUI);
    }
    public void UpdateResourceUI()
    {
        scrapTextObj.text = "Scrap: " + ((int)Scrap).ToString();

        lifeEssenceTextObj.text =  ((int)LifeEssence).ToString();
        arcaneEssenceTextObj.text = ((int)ArcaneEssence).ToString();
        emberEssenceTextObj.text = ((int)EmberEssence).ToString();
    }



    public bool TryBuildTower(float cost)
    {
        if (Scrap >= cost)
        {
            Scrap -= cost;
            return true;
        }
        return false;
    }
    public void AddScrap(float amount)
    {
        Scrap += amount;
    }

    public bool TryUpgradeTower(float cost, MagicType essenceType, MagicType chosenEssenceType)
    {
        if (PurchaseOptions(out bool[] purchaseOptions, cost))
        {
            if (essenceType == MagicType.Neutral)
            {
                if (chosenEssenceType == MagicType.Life && purchaseOptions[0])
                {
                    AddRemoveEssence(-cost, chosenEssenceType);
                    return true;
                }
                else if(chosenEssenceType == MagicType.Arcane && purchaseOptions[1])
                {
                    AddRemoveEssence(-cost, chosenEssenceType);
                    return true;
                }
                else if (chosenEssenceType == MagicType.Ember && purchaseOptions[2])
                {
                    AddRemoveEssence(-cost, chosenEssenceType);
                    return true;
                }
                return false;
            }


            else if (essenceType == MagicType.Life && purchaseOptions[0])
            {
                AddRemoveEssence(-cost, essenceType);
                return true;
            }
            else if (essenceType == MagicType.Arcane && purchaseOptions[1])
            {
                AddRemoveEssence(-cost, essenceType);
                return true;
            }
            else if (essenceType == MagicType.Ember && purchaseOptions[2])
            {
                AddRemoveEssence(-cost, essenceType);
                return true;
            }
        }
        return false;
    }
    public bool PurchaseOptions(out bool[] upgradesPossible, float cost)
    {
        upgradesPossible = new bool[3];

        if (lifeEssence >= cost)
        {
            upgradesPossible[0] = true;
        }
        if (arcaneEssence >= cost)
        {
            upgradesPossible[1] = true;
        }
        if (emberEssence >= cost)
        {
            upgradesPossible[2] = true;
        }
        return upgradesPossible.Contains(true);
    }

    public void AddRemoveEssence(float amount, MagicType type)
    {
        if (type == MagicType.Life)
        {
            LifeEssence += amount;
        }
        else if (type == MagicType.Arcane)
        {
            ArcaneEssence += amount;
        }
        else if (type == MagicType.Ember)
        {
            EmberEssence += amount;
        }
        else
        {
            LifeEssence += amount *= zeroPointThree;
            ArcaneEssence += amount *= zeroPointThree;
            EmberEssence += amount *= zeroPointThree;
        }
    }
}