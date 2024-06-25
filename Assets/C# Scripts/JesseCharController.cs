using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JesseCharController : MonoBehaviour
{
    private Animator anim;

    public AnimationPart[] animationParts;

    public float moveSpeed;
    public float rotSpeed;
    private float angle = 0;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W))
        {
            dir.y = 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            dir.x = -1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            dir.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            dir.x = 1;
        }
        transform.position += new Vector3(dir.x, 0, dir.y) * moveSpeed * Time.deltaTime;


        
        if (dir != Vector2.zero)
        {
            angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        }
        Quaternion toRotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, toRotation) < 3.5f)
        {
            transform.rotation = toRotation;
        }


        for (int i = 0; i < animationParts.Length; i++)
        {
            if (animationParts[i].playOnce)
            {
                animationParts[i].playOnce = false;
                anim.SetTrigger(animationParts[i].animationName);
                break;
            }
            anim.SetBool(animationParts[i].animationName, animationParts[i].loop);

            if (animationParts[i].loop)
            {
                break;
            }
        }
    }


    [System.Serializable]
    public class AnimationPart
    {
        public string animationName;
        public bool playOnce;
        public bool loop;
    }
}
