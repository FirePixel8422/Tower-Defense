using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCanon : TowerCore
{
    public bool splashUpgrade;


    public override void Shoot()
    {
        if (splashUpgrade)
        {
            projStats.doSplashDamage = true;
        }
        base.Shoot();
    }
}
