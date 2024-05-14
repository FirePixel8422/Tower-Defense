using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float size;

    public EnemyCore target;
    public float damage;
    public float speed;

    public void Init(EnemyCore _target, float _speed, float _damage)
    {
        target = _target;
        speed = _speed;
        damage = _damage;
        StartCoroutine(TrackTargetLoop());
    }


    private IEnumerator TrackTargetLoop()
    {
        while (true)
        {
            yield return null;
            if (target == null)
            {
                Destroy(gameObject);
                yield break;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.transform.position) < size)
            {
                target.Hit(damage);
                Destroy(gameObject);
                yield break;
            }
        }
    }
}
