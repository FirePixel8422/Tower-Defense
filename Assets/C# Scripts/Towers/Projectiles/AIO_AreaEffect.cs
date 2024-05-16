using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIO_AreaEffect : MonoBehaviour
{
    private Collider coll;
    private ProjectileStats projStats;

    private void Init(ProjectileStats _projStats)
    {
        coll = GetComponent<Collider>();
        projStats = _projStats;
        coll.enabled = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
