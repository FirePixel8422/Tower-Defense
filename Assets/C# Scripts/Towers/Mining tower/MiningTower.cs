using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningTower : TowerCore
{
    private EssenceManager essenceManager;

    public Material magicColormaterial;
    public Color[] magicColors;
    public float colorSwapSpeed;
    public int colorIndex;

    public MagicType generationType;
    public float generationPower;
    public float generatedEssence;



    public override void Start()
    {
        base.Start();
        magicColormaterial = transform.GetChild(0).GetComponent<Renderer>().material;
    }

    public override void Init()
    {
        essenceManager = EssenceManager.Instance;
    }
    public override void Shoot()
    {
        return;
    }

    public void ChangeGenerationType(MagicType newType)
    {
        colorIndex = (int)newType - 1;
        generationType = newType;
        generatedEssence = 0;
    }

    private void Update()
    {
        if (towerCompleted == false || generationType == MagicType.Neutral || essenceManager == null)
        {
            return;
        }

        if (towerPreviewRenderer.color != magicColors[colorIndex])
        {
            Color col = Color.Lerp(magicColormaterial.GetColor("_Base_Color"), magicColors[colorIndex], colorSwapSpeed * Time.deltaTime);
            magicColormaterial.SetColor("_Base_Color", col);
            towerPreviewRenderer.color = col;
        }

        generatedEssence += generationPower * Time.deltaTime;

        essenceManager.AddRemoveEssence((int)generatedEssence, generationType);

        generatedEssence -= (int)generatedEssence;
    }
}
