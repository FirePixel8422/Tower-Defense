using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningTower : TowerCore
{
    private EssenceManager essenceManager;

    public Material magicColormaterial;
    public Color[] magicColors;

    public MagicType generationType;
    public float generationPower;
    public float generatedEssence;



    public override void Init()
    {
        base.Init();
        essenceManager = EssenceManager.Instance;
    }

    public void ChangeGenerationType(MagicType newType)
    {
        generationType = newType;
        generatedEssence = 0;
    }

    private void Update()
    {
        if (towerCompleted == false || generationType == MagicType.Neutral || essenceManager == null)
        {
            return;
        }
        generatedEssence += generationPower * Time.deltaTime;

        essenceManager.AddRemoveEssence((int)generatedEssence, generationType);

        generatedEssence -= (int)generatedEssence;
    }
}
