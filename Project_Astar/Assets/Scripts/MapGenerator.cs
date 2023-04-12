using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static bool bPaintMode = false;

    public GroundTile prefGroundTile;
    public int width;
    public int height;
    public Vector2Int startLocation;
    public Vector2Int goalLocation;
    public float findPathInterval = .5f;
    public bool stepByStepMode = false;
    public bool waitAtAwake = false;

    private GroundTile[,] mGroundTiles;
    private int[,] mMap;

    private PriorityQueue<GroundTile> openedTiles;
    //private PriorityQueue<GroundTile> openedTilesQueue;
    private bool clicked = false;

    private void Start()
    {
        mGroundTiles = new GroundTile[width, height];
        mMap = new int[width, height];

        LoopMap((x, y) =>
        {
            mGroundTiles[x, y] = Instantiate<GroundTile>(prefGroundTile, new Vector3(x, 0, y), Quaternion.identity);

            var location = new Vector2Int(x, y);
            mGroundTiles[x, y].InitLocation(location);

            mMap[x, y] = 0;
        });

        mGroundTiles[startLocation.x, startLocation.y].Set(GroundTile.EType.Start);
        mGroundTiles[goalLocation.x, goalLocation.y].Set(GroundTile.EType.Goal);
        mGroundTiles[goalLocation.x, goalLocation.y].isGoal = true;

        mMap[startLocation.x, startLocation.y] = (int)GroundTile.EType.Start;
        mMap[goalLocation.x, goalLocation.y] = (int)GroundTile.EType.Goal;

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
                if (targetGroundTile.Type == GroundTile.EType.Wall)
                    targetGroundTile.Set(GroundTile.EType.Ground);
                else if (targetGroundTile.Type == GroundTile.EType.Ground)
                    targetGroundTile.Set(GroundTile.EType.Wall);
            }

            clicked = true;

            var x = targetGroundTile.Location.x;
            var y = targetGroundTile.Location.y;
            mMap[x, y] = (mMap[x, y] == 0) ? 1 : 0;
        };

        StartCoroutine(FindPath());
    }

    private IEnumerator FindPath()
    {
        if (waitAtAwake)
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        openedTiles = new PriorityQueue<GroundTile>(width * height);

        var currentLocation = new Vector2Int(startLocation.x, startLocation.y);
        GroundTile currentTile = mGroundTiles[currentLocation.x, currentLocation.y];

        currentTile.Open(null);
        OpenTiles(currentTile);

        while(openedTiles.Count > 0)
        {
            currentTile = openedTiles.Dequeue();

            yield return new WaitUntil(() => !stepByStepMode || clicked);
            clicked = false;

            if(currentTile.isGoal)
            {
                break;
            }

            currentTile.SetColor(GroundTile.EType.Wall);

            OpenTiles(currentTile);

            yield return new WaitUntil(() => !stepByStepMode || clicked);
            clicked = false;

            if(!stepByStepMode)
                yield return new WaitForSeconds(findPathInterval);
                
            currentTile.Close();
        }
    }

    private void OpenTiles(GroundTile currentTile)
    {
        var x = currentTile.Location.x;
        var y = currentTile.Location.y;
        GroundTile goalTile = mGroundTiles[goalLocation.x, goalLocation.y];

        // Up
        if (y + 1 < height && mGroundTiles[x, y + 1].isOpened == false)
        {
            GroundTile openTile = mGroundTiles[x, y + 1];
            openTile.Open(currentTile);
            openTile.costSoFar = (openTile.Type == GroundTile.EType.Wall) ? currentTile.costSoFar + 1000 : currentTile.costSoFar + 1;
            openTile.heuristic = Heuristic(goalTile, openTile);
            openedTiles.Enqueue(openTile, openTile.costSoFar + Heuristic(goalTile, openTile));
        }
        
        // Right
        if(x + 1 < width && mGroundTiles[x + 1, y].isOpened == false)
        {
            GroundTile openTile = mGroundTiles[x + 1, y];
            openTile.Open(currentTile);
            openTile.costSoFar = (openTile.Type == GroundTile.EType.Wall) ? currentTile.costSoFar + 1000 : currentTile.costSoFar + 1;
            openTile.heuristic = Heuristic(goalTile, openTile);
            openedTiles.Enqueue(openTile, openTile.costSoFar + Heuristic(goalTile, openTile));
        }
        
        // Down
        if(y - 1 >= 0 && mGroundTiles[x, y - 1].isOpened == false)
        {
            GroundTile openTile = mGroundTiles[x, y - 1];
            openTile.Open(currentTile);
            openTile.costSoFar = (openTile.Type == GroundTile.EType.Wall) ? currentTile.costSoFar + 1000 : currentTile.costSoFar + 1;
            openTile.heuristic = Heuristic(goalTile, openTile);
            openedTiles.Enqueue(openTile, openTile.costSoFar + Heuristic(goalTile, openTile));
        }
        
        // Left
        if(x - 1 >= 0 && mGroundTiles[x - 1, y].isOpened == false)
        {
            GroundTile openTile = mGroundTiles[x - 1, y];
            openTile.Open(currentTile);
            openTile.costSoFar = (openTile.Type == GroundTile.EType.Wall) ? currentTile.costSoFar + 1000 : currentTile.costSoFar + 1;
            openTile.heuristic = Heuristic(goalTile, openTile);
            openedTiles.Enqueue(openTile, openTile.costSoFar + Heuristic(goalTile, openTile));
        }
    }

    private int Heuristic(GroundTile goal, GroundTile target)
    {
        return Mathf.Abs(goal.Location.x - target.Location.x) + Mathf.Abs(goal.Location.y - target.Location.y);
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

    private void OnDrawGizmos() 
    {
        if(mMap != null)
        {
            for(int x = 0; x < width; ++x)
            {
                for(int y = 0; y < height; ++y)
                {
                    // Gizmos.color = (mMap[x, y] == 1) ? Color.black : Color.white;
                    // Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    // Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
