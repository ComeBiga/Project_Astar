using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingSimulator : MonoBehaviour
{
    public MapGenerator mMapGenerator;
    public PathFinder_Astar pathFinder;

    public float findingInterval = 0f;
    public bool stepByStepMode = false;
    public GroundTile.EType paintType = GroundTile.EType.Wall;

    private Map mMap;
    private bool bPaintMode = true;

    // Start is called before the first frame update
    void Start()
    {
        InitInput();

        mMap = mMapGenerator.Generate();
        pathFinder = new PathFinder_Astar(this, mMap);
        pathFinder.interval = findingInterval;
    }

    private void InitInput()
    {
        InputManager.Instance.onMouseLeftButtonDown += (screenCenterPos, rayCastHit) =>
        {
            if (rayCastHit.transform == null)
                return;

            var targetGroundTile = rayCastHit.transform.GetComponent<GroundTile>();

            if (bPaintMode)
                PaintTile(targetGroundTile);

            //Debug.Log($"");
            Debug.Log($"=====================================");
            Debug.Log($"Location:{targetGroundTile.Location}");
            Debug.Log($"Location:{targetGroundTile.Type}");
            Debug.Log($"Cost:{targetGroundTile.costSoFar}");
            Debug.Log($"Heuristic:{targetGroundTile.heuristic}");
            Debug.Log($"Cost + Heuristic:{targetGroundTile.costSoFar + targetGroundTile.heuristic}");
        };

        InputManager.Instance.OnKeyDown(KeyCode.C, () =>
        {
            if(!pathFinder.IsAvailable())
                return;

            mMap.Clear();

            bPaintMode = true;
        });

        InputManager.Instance.OnKeyDown(KeyCode.P, () =>
        {
            bPaintMode = !bPaintMode;
        });

        InputManager.Instance.OnKeyDown(KeyCode.Space, () =>
        {
            if (!pathFinder.IsAvailable())
                return;

            bPaintMode = false;

            if (stepByStepMode)
                pathFinder.FindPathStepByStep();
            else
                pathFinder.FindPath();
        });
    }

    private void PaintTile(GroundTile targetTile, GroundTile.EType _paintType = GroundTile.EType.Ground)
    {
        switch (_paintType)
        {
            case GroundTile.EType.Start:
                mMap.SetStart(targetTile);
                break;
            case GroundTile.EType.Goal:
                mMap.SetGoal(targetTile);
                break;
            default:
                mMap.Set(targetTile, _paintType);
                break;
        }
    }

    private void PaintTile(GroundTile targetTile)
    {
        PaintTile(targetTile, paintType);
    }

}
