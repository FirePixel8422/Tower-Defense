using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningTower : TowerCore
{
    private EssenceManager essenceManager;

    public GameObject[] waters;
    private Material[] magicColormaterial = new Material[2];

    public Color[] magicColors;
    public float colorSwapSpeed;
    public int colorIndex;

    public MagicType generationType;
    public float generationPower;
    public float generatedEssence;

    private Color materialColor;



    public override void Start()
    {
        base.Start();
        magicColormaterial[0] = waters[0].GetComponent<Renderer>().material;
        magicColormaterial[1] = waters[1].GetComponent<Renderer>().material;
        materialColor = magicColormaterial[0].GetColor("_Water_Color");
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
            materialColor = LinearColorLerp(materialColor, magicColors[colorIndex], colorSwapSpeed * Time.deltaTime);
            magicColormaterial[0].SetColor("_Water_Color", materialColor);
            magicColormaterial[1].SetColor("_Water_Color", materialColor);
            towerPreviewRenderer.color = materialColor;
        }

        generatedEssence += generationPower * Time.deltaTime;

        if((int)generatedEssence != 0)
        {
            essenceManager.AddRemoveEssence((int)generatedEssence, generationType);

            generatedEssence -= (int)generatedEssence;
        }
    }


    private Color LinearColorLerp(Color a, Color b, float maxStep)
    {
        a = new Color(
            Mathf.MoveTowards(a.r, b.r, maxStep),
            Mathf.MoveTowards(a.g, b.g, maxStep),
            Mathf.MoveTowards(a.b, b.b, maxStep),
            Mathf.MoveTowards(a.a, b.a, maxStep));
        return a;
    }
}
