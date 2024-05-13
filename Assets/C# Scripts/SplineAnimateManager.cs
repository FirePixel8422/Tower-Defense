using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class SplineAnimateManager : MonoBehaviour
{
    public static SplineAnimateManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    public float updateFps;
    public float moveSpeed;

    public WaitForSeconds wait;
    private void OnValidate()
    {
        wait = new WaitForSeconds(1 / updateFps);
    }


    public List<SplineAnimate> splineAnimators;
    public List<Vector3> cPos;


    private void Start()
    {
        wait = new WaitForSeconds(1 / updateFps);
        StartCoroutine(UpdateTransformLoop());
    }


    public void AddAnim(SplineAnimate anim)
    {
        splineAnimators.Add(anim);

        anim.UpdateTransform(out Vector3 pos);
        cPos.Add(pos);
    }

    private IEnumerator UpdateTransformLoop()
    {
        while (true)
        {
            yield return wait;
            for (int i = 0; i < splineAnimators.Count; i++)
            {
                if (splineAnimators[i] == null)
                {
                    splineAnimators.RemoveAt(i);
                    cPos.RemoveAt(i);

                    i -= 1;
                    continue;
                }
                splineAnimators[i].UpdateTransform(out Vector3 pos);
                cPos[i] = pos;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < splineAnimators.Count; i++)
        {
            if (splineAnimators[i] == null)
            {
                splineAnimators.RemoveAt(i);
                cPos.RemoveAt(i);
            }
            splineAnimators[i].transform.position = Vector3.MoveTowards(splineAnimators[i].transform.position, cPos[i], moveSpeed * Time.deltaTime);
        }
    }
}
