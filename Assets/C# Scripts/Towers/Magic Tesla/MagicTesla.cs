using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicTesla : TowerCore
{
    public VisualEffect effect;

    public override void Init()
    {
        base.Init();
        effect = GetComponent<VisualEffect>();
    }
    public override void Shoot()
    {
        anim.SetTrigger("Shoot");
        target.TryHit(projStats.damageType, projStats.damage);

        effect.Play();
    }
}
