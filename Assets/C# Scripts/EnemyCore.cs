using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCore : MonoBehaviour
{
    public float health;
    public float damage;

    public float moveSpeed;
    public float rotSpeed;

    public int pointIndex;
    public float progression;

    public MagicType[] elements;

    public MagicType immunityBarrier;
    public Material barrierShader;


    public void Init(ImmunityBarrier _immunityBarrier)
    {
        if (_immunityBarrier != ImmunityBarrier.None)
        {
            if (_immunityBarrier == ImmunityBarrier.Smart)
            {
                immunityBarrier = TowerManager.Instance.HighestMagicType();
            }
            barrierShader.GameObject().SetActive(true);
        }
    }
}
