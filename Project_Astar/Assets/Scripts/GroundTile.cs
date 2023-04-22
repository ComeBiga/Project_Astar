using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    public enum EType
    {
        Ground = 0,
        Wall = 1,
        Selected = 97,
        Closed = 98,
        Opened = 99,
        Start = 100,
        Goal = 101
    }

    [Header("ColorMaterials")]
    public Material matGround;
    public Material matWall;
    public Material matSelected;
    public Material matClosed;
    public Material matOpened;
    public Material matStart;
    public Material matGoal;

    public Vector2Int Location => mlocation;
    public EType Type => mType;
    public GroundTile PreviousTile => mPreviousTile;
    public bool IsGround => mType == EType.Ground;

    [HideInInspector]
    public int costSoFar = 0;
    [HideInInspector]
    public float heuristic = 0f;
    [HideInInspector]
    public bool isOpened = false;

    private Vector2Int mlocation;
    private EType mType;
    private GroundTile mPreviousTile;

    private MeshRenderer mMeshRenderer;

    public void Init(Vector2Int location, EType type = EType.Ground)
    {
        Set(EType.Ground);

        InitLocation(location);
        Reset();
    }

    public void Reset()
    {
        Set(mType);

        costSoFar = 0;
        heuristic = 0;
        mPreviousTile = null;

        isOpened = false;
    }

    public void InitLocation(Vector2Int location)
    {
        mlocation = location;
        mType = EType.Ground;
    }

    public void Set(EType type)
    {
        mType = type;

        SetColor(type);
    }

    public void Open(GroundTile previousTile)
    {
        isOpened = true;
        mPreviousTile = previousTile;

        if(IsGround)
            SetColor(EType.Opened);
    }

    public void Close()
    {
        if(IsGround)
            SetColor(EType.Closed);
    }

    public void Select()
    {
        //if (mType != EType.Start && mType != EType.Goal && mType != EType.Wall)
        if(IsGround)
            SetColor(EType.Selected);
    }

    public void SetColor(EType type)
    {
        switch(type)
        {
            case EType.Ground:
                mMeshRenderer.material = matGround;
                break;
            case EType.Wall:
                mMeshRenderer.material = matWall;
                break;
            case EType.Selected:
                mMeshRenderer.material = matSelected;
                break;
            case EType.Closed:
                mMeshRenderer.material = matClosed;
                break;
            case EType.Opened:
                mMeshRenderer.material = matOpened;
                break;
            case EType.Start:
                mMeshRenderer.material = matStart;
                break;
            case EType.Goal:
                mMeshRenderer.material = matGoal;
                break;
            default:
                UnityEngine.Assertions.Assert.IsTrue(false);
                break;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        mMeshRenderer = GetComponent<MeshRenderer>();

    }
}
