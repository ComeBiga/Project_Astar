using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    public enum EType
    {
        Ground = 0,
        Wall = 1,
        Closed = 98,
        Opened = 99,
        Start = 100,
        Goal = 101
    }

    public Material[] tileColorMaterials;
    public Vector2Int Location => mlocation;
    public EType Type => mType;
    public GroundTile PreviousTile => mPreviousTile;

    [HideInInspector]
    public int costSoFar = 0;
    [HideInInspector]
    public int heuristic = 0;
    [HideInInspector]
    public bool isOpened = false;
    [HideInInspector]
    public bool isGoal = false;

    private Vector2Int mlocation;
    private EType mType;
    private GroundTile mPreviousTile;

    private MeshRenderer mMeshRenderer;
    private int mTileColorIndex = 0;

    public void Init(Vector2Int location, EType type = EType.Ground)
    {
        InitLocation(location);
        Reset();
    }

    public void Reset()
    {
        Set(EType.Ground);

        costSoFar = 0;
        heuristic = 0;
        mPreviousTile = null;

        isOpened = false;
        isGoal = false;
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

        if(mType != EType.Start && mType != EType.Goal && mType != EType.Wall)
            SetColor(EType.Opened);
    }

    public void Close()
    {
        if(mType != EType.Start && mType != EType.Goal && mType != EType.Wall)
            SetColor(EType.Closed);
    }

    public void SetColor(EType type)
    {
        switch(type)
        {
            case EType.Ground:
                mMeshRenderer.material = tileColorMaterials[0];
                break;
            case EType.Wall:
                mMeshRenderer.material = tileColorMaterials[1];
                break;
            case EType.Closed:
                mMeshRenderer.material = tileColorMaterials[2];
                break;
            case EType.Opened:
                mMeshRenderer.material = tileColorMaterials[3];
                break;
            case EType.Start:
                mMeshRenderer.material = tileColorMaterials[4];
                break;
            case EType.Goal:
                mMeshRenderer.material = tileColorMaterials[5];
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
