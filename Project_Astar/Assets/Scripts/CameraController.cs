using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance => instance;

    public float moveSpeed = 1f;
    public float mouseSensitivity = 100.0f;
    public int frameRate = 144;
    public bool bLockMovement = false;
    public bool bLockRotation = false;

    [SerializeField]
    private GameObject goAimPoint;

    private Camera mCamera;
    private float mRotationX = 0f;
    private float mRotationY = 0f;

    public void HideAimPoint(bool value)
    {
        goAimPoint.SetActive(!value);
    }

    public void LockCamera(bool value)
    {
        bLockRotation = value;
        bLockMovement = value;

        Cursor.visible = value;
        Cursor.lockState = (value == true) ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    private void Awake() 
    {
        if(instance == null)
            instance = this;

        mCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = frameRate;
    }

    private void LateUpdate()
    {
        // Camera Movement
        if(!bLockMovement)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 translation = forward * vertical + right * horizontal;

            mCamera.transform.Translate(translation * moveSpeed, Space.World);
        }

        // Camera Rotation
        if (!bLockRotation)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            mRotationX -= mouseY;
            mRotationX = Mathf.Clamp(mRotationX, -90.0f, 90.0f); // 수직 회전 제한
            mRotationY += mouseX;

            transform.rotation = Quaternion.Euler(mRotationX, mRotationY, 0.0f);
        }
    }
}
