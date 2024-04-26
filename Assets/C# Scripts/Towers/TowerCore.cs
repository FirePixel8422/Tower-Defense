using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TowerCore : MonoBehaviour
{
    public Material mat;
    public float active;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        active -= Time.deltaTime * speed;
        mat.SetFloat("_Active", active);
    }
}
