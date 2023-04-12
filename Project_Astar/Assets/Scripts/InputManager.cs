using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    public static InputManager Instance => instance;

    public event Action<Vector2, RaycastHit> onMouseButtonDown;

    private Vector2 mScreenCenter;
    private int mScreenCenterX;
    private int mScreenCenterY;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        mScreenCenterX = Camera.main.pixelWidth / 2;
        mScreenCenterY = Camera.main.pixelHeight / 2;

        mScreenCenter = new Vector2(mScreenCenterX, mScreenCenterY);
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 1f));

            Physics.Raycast(ray, out RaycastHit hit, 500f);

            onMouseButtonDown?.Invoke(mScreenCenter, hit);
            //Debug.Log(mScreenCenter);
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            MapGenerator.bPaintMode = !MapGenerator.bPaintMode;
        }
    }
}
