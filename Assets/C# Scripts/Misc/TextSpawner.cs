using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using TMPro;


public class TextSpawner : MonoBehaviour
{
    public static TextSpawner Instance;
    private void Awake()
    {
        Instance = this;
    }




    public GameObject textPrefab;

    public TextMesh SpawnTextMesh(Vector3 position, Quaternion rotation, string text, Color color, float size, float time)
    {
        TextMesh textObj = Instantiate(textPrefab).GetComponent<TextMesh>();

        textObj.transform.SetPositionAndRotation(position, rotation);

        textObj.text = text;
        textObj.color = color;
        textObj.characterSize = size;

        StartCoroutine(DespawnText(textObj.gameObject, time));

        return textObj;
    }
    private IEnumerator DespawnText(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}