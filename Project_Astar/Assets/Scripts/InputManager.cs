using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    public static InputManager Instance => instance;

    public event Action<Vector2, RaycastHit> onMouseLeftButtonDown;

    public event Action<Vector2, RaycastHit> onMouseMiddleButton;
    public event Action<Vector2, RaycastHit> onMouseMiddleButtonDown;
    public event Action<Vector2, RaycastHit> onMouseMiddleButtonUp;

    private Vector2 mScreenCenter;
    private int mScreenCenterX;
    private int mScreenCenterY;

    private Dictionary<KeyCode, Action> mKeyDownActionDic = new Dictionary<KeyCode, Action>(50);

    public void OnKeyDown(KeyCode keyCode, Action action)
    {
        if(!mKeyDownActionDic.TryGetValue(keyCode, out Action keyDownAction))
        {
            mKeyDownActionDic.Add(keyCode, action);
            return;
        }

        keyDownAction += action;
    }

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

            onMouseLeftButtonDown?.Invoke(mScreenCenter, hit);
        }

        if(Input.GetMouseButtonDown(2))
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 1f));
            Physics.Raycast(ray, out RaycastHit hit, 500f);

            onMouseMiddleButtonDown?.Invoke(mScreenCenter, hit);
        }
        
        if(Input.GetMouseButton(2))
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 1f));
            Physics.Raycast(ray, out RaycastHit hit, 500f);

            onMouseMiddleButton?.Invoke(mScreenCenter, hit);
        }
        
        if(Input.GetMouseButtonUp(2))
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 1f));
            Physics.Raycast(ray, out RaycastHit hit, 500f);

            onMouseMiddleButtonUp?.Invoke(mScreenCenter, hit);
        }

        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    MapGenerator.bPaintMode = !MapGenerator.bPaintMode;
        //}

        foreach (var keyDownAction in mKeyDownActionDic)
        {
            if(Input.GetKeyDown(keyDownAction.Key))
            {
                keyDownAction.Value.Invoke();
            }
        }
    }
}
