using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Serialization;
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

    private const float zeroPointThree = 1 / 3;


    private void Start()
    {
        OnEssenceChanged.AddListener(UpdateEssenceUI);
    }
    public void UpdateEssenceUI()
    {
        lifeEssenceTextObj.text =  ((int)LifeEssence).ToString();
        arcaneEssenceTextObj.text = ((int)ArcaneEssence).ToString();
        emberEssenceTextObj.text = ((int)EmberEssence).ToString();
    }



    public bool TryPurchase(float cost, MagicType essenceType, MagicType chosenEssenceType)
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
            EmberEssence += amount *= zeroPointThree;
            ArcaneEssence += amount *= zeroPointThree;
            LifeEssence += amount *= zeroPointThree;
        }
    }

    public void GenerateEssenceFromEnemy(float amount, MagicType type)
    {
        if (type == MagicType.Neutral)
        {
            LifeEssence += amount *= zeroPointThree;
            ArcaneEssence += amount *= zeroPointThree;
            EmberEssence += amount *= zeroPointThree;
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