using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class EssenceManager : MonoBehaviour
{
    public static EssenceManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    private int lifeEssence;
    public int LifeEssence
    {
        get
        {
            return lifeEssence;
        }
        set
        {
            lifeEssence = value;
            OnEssenceChanged.Invoke();
        }
    }
    private int arcaneEssence;
    public int ArcaneEssence
    {
        get
        {
            return arcaneEssence;
        }
        set
        {
            arcaneEssence = value;
            OnEssenceChanged.Invoke();
        }
    }
    private int emberEssence;
    public int EmberEssence
    {
        get
        {
            return emberEssence;
        }
        set
        {
            emberEssence = value;
            OnEssenceChanged.Invoke();
        }
    }


    public UnityEvent OnEssenceChanged;

    public TextMeshProUGUI lifeEssenceTextObj;
    public TextMeshProUGUI arcaneEssenceTextObj;
    public TextMeshProUGUI emberEssenceTextObj;



    private void Start()
    {
        OnEssenceChanged.AddListener(UpdateEssenceUI);
    }
    public void UpdateEssenceUI()
    {
        lifeEssenceTextObj.text = lifeEssence.ToString();
        arcaneEssenceTextObj.text = arcaneEssence.ToString();
        emberEssenceTextObj.text = emberEssence.ToString();
    }

    public bool[] AllPossibleUpgradeOptions(int amount)
    {
        bool[] upgradesPossible = new bool[3];

        if (lifeEssence >= amount)
        {
            upgradesPossible[0] = true;
        }
        if (arcaneEssence >= amount)
        {
            upgradesPossible[1] = true;
        }
        if (emberEssence >= amount)
        {
            upgradesPossible[2] = true;
        }
        return upgradesPossible;
    }
    public bool UpgradePossibleWithType(int amount, MagicType type)
    {
        bool[] upgradesPossible = AllPossibleUpgradeOptions(amount);

        if (type == MagicType.Life && upgradesPossible[0])
        {
            return true;
        }
        if (type == MagicType.Arcane && upgradesPossible[1])
        {
            return true;
        }
        if (type == MagicType.Ember && upgradesPossible[2])
        {
            return true;
        }
        return upgradesPossible.Contains(true);
    }

    public void AddRemoveEssence(int amount, MagicType type)
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
    }
}