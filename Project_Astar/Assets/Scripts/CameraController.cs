using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float mouseSensitivity = 100.0f;
    public int frameRate = 144;

    private Camera mCamera;
    private float mRotationX = 0f;
    private float mRotationY = 0f;

    private void Awake() 
    {
        mCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = frameRate;
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 translation = forward * vertical + right * horizontal;

        mCamera.transform.Translate(translation * moveSpeed, Space.World);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        mRotationX -= mouseY;
        mRotationX = Mathf.Clamp(mRotationX, -90.0f, 90.0f); // 수직 회전 제한
        mRotationY += mouseX;

        transform.rotation = Quaternion.Euler(mRotationX, mRotationY, 0.0f);

        //Quaternion rotation = Quaternion.Euler(mRotationX, mRotationY, 0.0f);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime /* x speed */);
        //transform.Rotate(Vector3.up * mouseX);
    }
}
