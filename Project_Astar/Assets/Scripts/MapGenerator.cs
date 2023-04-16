using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private bool bPaintMode = true;

    public GroundTile prefGroundTile;
    public int width;
    public int height;
    public Vector2Int startLocation;
    public Vector2Int goalLocation;
    public float findPathInterval = .5f;
    public bool stepByStepMode = false;
    public bool waitAtAwake = false;
    public GroundTile.EType paintType = GroundTile.EType.Wall;

    private GroundTile[,] mGroundTiles;

    private PriorityQueue<GroundTile> openedTiles;
    private bool clicked = false;

    private GroundTile mStartTile = null;
    private GroundTile mGoalTile = null;

    private const int COST_WALL = 1000;
    private const int COST_GROUND = 1;

    public void ClearMap()
    {
        LoopMap((x, y) =>
        {
            mGroundTiles[x, y].Reset();
        });

        bPaintMode = true;
    }

    private void Start()
    {
        InstantiateGroundTiles(width, height);

        InputManager.Instance.onMouseButtonDown += (screenCenterPos, rayCastHit) =>
        {
            if(rayCastHit.transform == null)
                return;

            var targetGroundTile = rayCastHit.transform.GetComponent<GroundTile>();

            //Debug.Log($"");
            Debug.Log($"=====================================");
            Debug.Log($"Location:{targetGroundTile.Location}");
            Debug.Log($"Location:{targetGroundTile.Type}");
            Debug.Log($"Cost:{targetGroundTile.costSoFar}");
            Debug.Log($"Heuristic:{targetGroundTile.heuristic}");
            Debug.Log($"Cost + Heuristic:{targetGroundTile.costSoFar + targetGroundTile.heuristic}");

            if (bPaintMode)
            {
                PaintTile(targetGroundTile);
            }

            clicked = true;

            var x = targetGroundTile.Location.x;
            var y = targetGroundTile.Location.y;
        };

        InputManager.Instance.OnKeyDown(KeyCode.C, () =>
        {
            ClearMap();
        });
        
        InputManager.Instance.OnKeyDown(KeyCode.P, () =>
        {
            bPaintMode = !bPaintMode;
        });
        
        InputManager.Instance.OnKeyDown(KeyCode.Space, () =>
        {
            if(mStartTile == null || mGoalTile == null)
                return;

            if(stepByStepMode)
                StartCoroutine(FindPathStepByStep());
            else
                StartCoroutine(FindPath());
        });
    }

    private IEnumerator FindPath()
    {
        bPaintMode = false;
        openedTiles = new PriorityQueue<GroundTile>(width * height);

        GroundTile currentTile = mStartTile;

        currentTile.Open(null);
        OpenTiles(currentTile);

        while(openedTiles.Count > 0)
        {
            currentTile = openedTiles.Dequeue();

            if(currentTile.Type == GroundTile.EType.Goal)
            {
                break;
            }

            currentTile.SetColor(GroundTile.EType.Wall);

            OpenTiles(currentTile);

            yield return new WaitForSeconds(findPathInterval);
                
            currentTile.Close();
        }

        GroundTile previousTile = currentTile.PreviousTile;

        while (previousTile != null)
        {
            previousTile.SetColor(GroundTile.EType.Goal);

            previousTile = previousTile.PreviousTile;
        }
    }

    private IEnumerator FindPathStepByStep()
    {
        bPaintMode = false;
        openedTiles = new PriorityQueue<GroundTile>(width * height);

        GroundTile currentTile = mStartTile;

        currentTile.Open(null);
        OpenTiles(currentTile);

        while (openedTiles.Count > 0)
        {
            currentTile = openedTiles.Dequeue();

            yield return new WaitUntil(() => clicked);
            clicked = false;

            if (currentTile.Type == GroundTile.EType.Goal)
            {
                break;
            }

            currentTile.SetColor(GroundTile.EType.Wall);

            OpenTiles(currentTile);

            yield return new WaitUntil(() => clicked);
            clicked = false;

            if (!stepByStepMode && findPathInterval > 0f)
                yield return new WaitForSeconds(findPathInterval);

            currentTile.Close();
        }

        GroundTile previousTile = currentTile.PreviousTile;

        while (previousTile != null)
        {
            previousTile.SetColor(GroundTile.EType.Goal);

            previousTile = previousTile.PreviousTile;
        }
    }

    private void OpenTiles(GroundTile currentTile)
    {
        var x = currentTile.Location.x;
        var y = currentTile.Location.y;

        //GroundTile openTile;

        // Up
        OpenTile(x, y + 1, currentTile);

        // Right
        OpenTile(x + 1, y, currentTile);

        // Down
        OpenTile(x, y - 1, currentTile);

        // Left
        OpenTile(x - 1, y, currentTile);
    }

    private void OpenTile(int x, int y, GroundTile previousTile)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        GroundTile targetTile = mGroundTiles[x, y];

        if (targetTile.Type == GroundTile.EType.Wall || targetTile.isOpened)
            return;

        targetTile.Open(previousTile);
        targetTile.costSoFar = previousTile.costSoFar + GetCostSoFar(targetTile);
        targetTile.heuristic = GetHeuristic(mGoalTile, targetTile);

        openedTiles.Enqueue(targetTile, targetTile.costSoFar + GetHeuristic(mGoalTile, targetTile));
    }

    private int GetCostSoFar(GroundTile openTile)
    {
        switch(openTile.Type)
        {
            case GroundTile.EType.Wall:
                return COST_WALL;
            default:
                return COST_GROUND;
        }
    }

    private int GetHeuristic(GroundTile goal, GroundTile target)
    {
        return Mathf.Abs(goal.Location.x - target.Location.x) + Mathf.Abs(goal.Location.y - target.Location.y);
    }

    private void InstantiateGroundTiles(int _width, int _height)
    {
        mGroundTiles = new GroundTile[_width, _height];

        LoopMap((x, y) =>
        {
            mGroundTiles[x, y] = Instantiate<GroundTile>(prefGroundTile, new Vector3(x, 0, y), Quaternion.identity);

            var location = new Vector2Int(x, y);
            mGroundTiles[x, y].Init(location);
        });
    }

    private void PaintTile(GroundTile targetTile, GroundTile.EType _paintType = GroundTile.EType.Ground)
    {
        switch(_paintType)
        {
            case GroundTile.EType.Start:
                mStartTile?.Set(GroundTile.EType.Ground);
                mStartTile = targetTile;
                break;
            case GroundTile.EType.Goal:
                mGoalTile?.Set(GroundTile.EType.Ground);
                mGoalTile = targetTile;
                break;
            default:
                { 
                    if (targetTile.Type == GroundTile.EType.Start)
                        mStartTile = null;

                    if (targetTile.Type == GroundTile.EType.Goal)
                        mGoalTile = null;
                }
                break;
        }

        targetTile.Set(_paintType);
    }

    private void PaintTile(GroundTile targetTile)
    {
        PaintTile(targetTile, paintType);
    }

    private void LoopMap(System.Action<int, int> action)
    {
        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
            {
                action.Invoke(x, y);
            }
        }
    }
}
