using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public AnimationCurve zoom_RotationCurve;
    public AnimationCurve zoom_PositionCurve;

    public Transform camCenter;
    public Transform cam;
    public Transform worldCenter;

    public float centerRotSpeed;
    public float moveSpeed;
    public float zoomSpeed;

    public float scrollForMaxZoom;
    public float cScroll;

    private float centerRotY;
    private Vector3 camMoveDir;

    public bool animatePanChanged;
        

    //detect input for reset of camera
    public void OnResetCam(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            StartCoroutine(ResetCamera());
        }
    }

    //detect scroll input for zooming in and out on the field
    public void OnScroll(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Scroll(ctx.ReadValue<Vector2>().y);
        }
    }
    //detect Q/E Input for rotation of Camera left and right
    public void OnRotate(InputAction.CallbackContext ctx)
    {
        centerRotY = ctx.ReadValue<Vector2>().x;
    }
    //detect WASD movement Input for movement of the camera
    public void OnMove(InputAction.CallbackContext ctx)
    {
        camMoveDir = ctx.ReadValue<Vector3>();
    }


    private void Update()
    {
        // check for inputs this frame
        if (Mathf.Abs(centerRotY) > 0.01f)
        {
            RotateCam();
        }

        if (camMoveDir.sqrMagnitude > 0.01f)
        {
            MoveCam();
        }

        if (animatePanChanged)
        {
            AnimatePanCam();
        }
    }

    private IEnumerator ResetCamera()
    {
        while (true)
        {
            yield return null;
            camCenter.rotation = Quaternion.RotateTowards(camCenter.rotation, Quaternion.identity, centerRotSpeed);
        }
    }

    //rotate camera horizontally around middlePoint with Q/E
    private void RotateCam()
    {
        camCenter.Rotate(0, centerRotY * centerRotSpeed * Time.deltaTime, 0);
    }

    //move camCenterPoint with Directional WASD Input, because camCenter moves, the rotation center point for horizontally rotating changes too.
    private void MoveCam()
    {
        Vector3 targetPosition = camCenter.localPosition + camCenter.TransformDirection(camMoveDir);
        camCenter.localPosition = Vector3.MoveTowards(camCenter.localPosition, targetPosition, moveSpeed * Time.deltaTime);
    }

    //move camera down/up rotating upwards/downwards following a smooth animation curve
    private void AnimatePanCam()
    {
        Vector3 camRot = cam.localEulerAngles;
        camRot.x = zoom_RotationCurve.Evaluate(cScroll);

        Vector3 camPos = cam.localPosition;
        camPos.y = zoom_PositionCurve.Evaluate(cScroll);

        cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(camRot), zoomSpeed * Time.deltaTime);
        cam.localPosition = Vector3.Lerp(cam.localPosition, camPos, zoomSpeed * Time.deltaTime);

        if(cam.localPosition == camPos && cam.localRotation == Quaternion.Euler(camRot))
        {
            animatePanChanged = false;
        }
    }

    //use scroll input and add that to cScroll float clamp between 0 and 1 for the animationCurve
    private void Scroll(float scrollDelta)
    {
        scrollDelta /= 120; // 120 = 1 scroll move
        cScroll = Mathf.Clamp(cScroll + scrollDelta / scrollForMaxZoom, 0, 1);

        animatePanChanged = true;
    }
}
