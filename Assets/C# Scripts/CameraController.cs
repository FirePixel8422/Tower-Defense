using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public AnimationCurve c;


    public Transform camCenter;
    public Transform cam;
    public Transform worldCenter;

    public float rotSpeed;
    public float scrollSpeed;

    public int xAmount;
    public float[] dir;

    public Vector3 camPos;



    public void OnScroll(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Scroll(ctx.ReadValue<Vector2>().y);
        }
    }

    private void Start()
    {
        dir = new float[xAmount];
    }

    private void Update()
    {
        for (int x = 0; x < xAmount; x++)
        {
            dir[x] = Mathf.Clamp01(c.Evaluate(1 / (float)xAmount * x));
        }

        float roty = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            roty -= 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            roty += 1;
        }
        camCenter.Rotate(new Vector3(0, roty * rotSpeed * Time.deltaTime, 0));
    }

    private void Scroll(float scrollDelta)
    {
        print(scrollDelta);
    }
}
