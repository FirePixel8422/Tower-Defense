using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }
}
