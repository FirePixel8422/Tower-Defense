using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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

    public float centerRotY;
    public Vector3 camMoveDir;



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
        //rotate camera horizontally around middlePoint with Q/E
        camCenter.Rotate(new Vector3(0, centerRotY * centerRotSpeed * Time.deltaTime, 0));

        //move camCenterPoint with Directional WASD Input, because camCenter moves, the rotation center point for horizontally rotating changes too.
        camCenter.localPosition = Vector3.MoveTowards(camCenter.localPosition, camCenter.localPosition + camCenter.TransformDirection(camMoveDir), moveSpeed * Time.deltaTime);


        # region Move camera down/up rotating upwards/downwards following a smooth animation curve

        Vector3 camRot = cam.localEulerAngles;
        camRot.x = zoom_RotationCurve.Evaluate(cScroll);

        Vector3 camPos = cam.localPosition;
        camPos.y = zoom_PositionCurve.Evaluate(cScroll);

        cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(camRot), zoomSpeed * Time.deltaTime);

        cam.localPosition = Vector3.Lerp(cam.localPosition, camPos, zoomSpeed * Time.deltaTime);
        #endregion
    }

    private void Scroll(float scrollDelta)
    {
        //use scroll input and add that to currentScroll float clamped between 0 and 1 for the animation curve percentages
        //120 = 1 scroll move
        scrollDelta /= 120;
        cScroll = Mathf.Clamp(cScroll + scrollDelta / scrollForMaxZoom, 0, 1);
    }
}
