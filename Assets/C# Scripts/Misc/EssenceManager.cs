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
            OnEssenceChanged.Invoke();
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
            OnEssenceChanged.Invoke();
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

        if(UpgradePossibleWithType(out bool[] options, 25, MagicType.Ember))
        {
            for (int i = 0; i < options.Length; i++)
            {
                print(i + " = " + options[i]);
            }
        }
    }
    public void UpdateEssenceUI()
    {
        lifeEssenceTextObj.text =  ((int)LifeEssence).ToString();
        arcaneEssenceTextObj.text = ((int)ArcaneEssence).ToString();
        emberEssenceTextObj.text = ((int)EmberEssence).ToString();
    }

    public bool[] AllPossibleUpgradeOptions(float amount)
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
    public bool UpgradePossibleWithType(out bool[] options, float amount, MagicType type)
    {
        options = AllPossibleUpgradeOptions(amount);

        if (type == MagicType.Life && options[0])
        {
            return true;
        }
        if (type == MagicType.Arcane && options[1])
        {
            return true;
        }
        if (type == MagicType.Ember && options[2])
        {
            return true;
        }
        return options.Contains(true);
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
            EmberEssence += amount / 3;
            ArcaneEssence += amount / 3;
            LifeEssence += amount / 3;
        }
    }

    public void GenerateEssenceFromEnemy(float amount, MagicType type)
    {
        if (type == MagicType.Neutral)
        {
            LifeEssence += amount / 3;
            ArcaneEssence += amount / 3;
            EmberEssence += amount / 3;
        }
        else if (type == MagicType.Life)
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