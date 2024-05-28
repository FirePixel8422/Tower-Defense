using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfusionEffect : AIO_AreaEffect
{
    public override void OnHitTarget(EnemyCore target)
    {
        base.OnHitTarget(target);
        target.moveSpeed *= -1;
    }
}
