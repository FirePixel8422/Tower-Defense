using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private void Awake()
    {
        Instance = this;
    }


    public AnimationCurve zoom_RotationCurve;
    public AnimationCurve zoom_PositionCurve;

    public Transform camCenter;
    public Transform cam;
    public Transform worldCenter;

    public float rotSpeed;
    public float mouseSens = 1;

    public float moveSpeed;

    public Vector3 camConfinementOffset;
    public Vector3 camConfinementSize;
    public float zoomSpeed;

    public float scrollForMaxZoom;
    public float cScroll;

    private float centerRotY;
    private Vector3 camMoveDir;

    private bool animatePanChanged;

    public bool altRotating;
    public bool control;



    //detect input for reset of camera
    public void OnResetCam(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (control && altRotating == false)
            {
                camMoveDir = Vector3.zero;
                centerRotY = 0;
                control = false;
                StartCoroutine(ResetCamera());
            }
        }
    }

    //detect scroll input for zooming in and out on the field
    public void OnScroll(InputAction.CallbackContext ctx)
    {
        if (control)
        {
            if (ctx.performed)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;

                var results = new List<RaycastResult>();
                SelectionManager.gfxRayCaster.Raycast(pointerEventData, results);

                if (results.Count > 0)
                {
                    return;
                }

                Scroll(ctx.ReadValue<Vector2>().y);
            }
        }
    }

    //detect Q/E Input for rotation of Camera left and right
    public void OnRotate(InputAction.CallbackContext ctx)
    {
        centerRotY = ctx.ReadValue<Vector2>().x;
    }

    //hold a key and swipe with mouse to rotate
    public void OnRotateAltHeld(InputAction.CallbackContext ctx)
    {
        altRotating = ctx.ReadValueAsButton();
    }

    //detect WASD movement Input for movement of the camera
    public void OnMove(InputAction.CallbackContext ctx)
    {
        camMoveDir = ctx.ReadValue<Vector3>();
    }

    //if key is held camera moves twice as fast
    public void OnFastMoveHeld(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            moveSpeed *= 2;
        }
        if (ctx.canceled)
        {
            moveSpeed /= 2;
        }
    }

    //update mouse sensitivity
    public void UpdateMouseSens(float newSens)
    {
        mouseSens = newSens;
    }


    private void Update()
    {
        if (control == true)
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
        if (altRotating)
        {
            camCenter.Rotate(0, Input.GetAxis("Mouse X") * -rotSpeed * mouseSens * Time.deltaTime, 0);
        }
    }

    private IEnumerator ResetCamera()
    {
        float _rotSpeed = Mathf.Abs(camCenter.localRotation.y) * 360;
        float _moveSpeed = camCenter.localPosition.magnitude * 2;
        while (true)
        {
            yield return null;

            camCenter.localRotation = Quaternion.RotateTowards(camCenter.localRotation, Quaternion.identity, _rotSpeed * Time.deltaTime);

            camCenter.localPosition = Vector3.MoveTowards(camCenter.localPosition, Vector3.zero, _moveSpeed * Time.deltaTime);

            animatePanChanged = true;
            cScroll = 0;


            if (camCenter.localRotation == Quaternion.identity && camCenter.localPosition == Vector3.zero)
            {
                control = true;
                yield break;
            }
        }
    }

    //rotate camera horizontally around middlePoint with Q/E
    private void RotateCam()
    {
        camCenter.Rotate(0, centerRotY * -rotSpeed * Time.deltaTime, 0);
    }

    //move camCenterPoint with Directional WASD Input, because camCenter moves, the rotation center point for horizontally rotating changes too.
    private void MoveCam()
    {
        Vector3 targetPosition = camCenter.localPosition + camCenter.TransformDirection(camMoveDir);
        camCenter.localPosition = Vector3.MoveTowards(camCenter.localPosition, targetPosition, -moveSpeed * Time.deltaTime);

        camCenter.localPosition = VectorLogic.Clamp(camCenter.localPosition, -camConfinementSize, camConfinementSize);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + camConfinementOffset, camConfinementSize);
    }
}
