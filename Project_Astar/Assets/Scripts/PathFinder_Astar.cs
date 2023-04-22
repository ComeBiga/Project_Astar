using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder_Astar
{
    public float interval = 0f;

    private MonoBehaviour mMonoBehaviour;
    private Map mMap;
    private PriorityQueue<GroundTile> openedTilePriorityQueue;

    private bool finding = false;

    private const float HEURISTIC_ADJUST = .001f;

    public PathFinder_Astar(MonoBehaviour monoBehaviour, Map map)
    {
        mMonoBehaviour = monoBehaviour;
        mMap = map;

        openedTilePriorityQueue = new PriorityQueue<GroundTile>(map.Width * map.Height);
    }

    public bool IsAvailable()
    {
        if (finding || mMap.StartTile == null || mMap.GoalTile == null)
            return false;

        return true;
    }

    public void FindPath()
    {
        mMonoBehaviour.StartCoroutine(EFindPath());
    }

    public void FindPathStepByStep()
    {
        mMonoBehaviour.StartCoroutine(EFindPathStepByStep());
    }

    private IEnumerator EFindPath()
    {
        finding = true;
        openedTilePriorityQueue.Clear();

        GroundTile currentTile = mMap.StartTile;
        currentTile.Open(null);
        OpenTiles(currentTile);

        while (openedTilePriorityQueue.Count > 0)
        {
            currentTile = openedTilePriorityQueue.Dequeue();

            if (currentTile.Type == GroundTile.EType.Goal)
                break;

            OpenTiles(currentTile);

            yield return new WaitForSeconds(interval);

            currentTile.Close();
        }

        DrawPath(currentTile);
        finding = false;
    }

    private IEnumerator EFindPathStepByStep()
    {
        finding = true;
        openedTilePriorityQueue.Clear();

        GroundTile currentTile = mMap.StartTile;
        currentTile.Open(null);
        OpenTiles(currentTile);

        while (openedTilePriorityQueue.Count > 0)
        {
            currentTile = openedTilePriorityQueue.Dequeue();

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            if (currentTile.Type == GroundTile.EType.Goal)
                break;

            OpenTiles(currentTile);

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            if (interval > 0f)
                yield return new WaitForSeconds(interval);

            currentTile.Close();
        }

        DrawPath(currentTile);
        finding = false;
    }

    private void OpenTiles(GroundTile currentTile)
    {
        currentTile.Select();

        var x = currentTile.Location.x;
        var y = currentTile.Location.y;

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
        if (!mMap.TryGetGroundTile(x, y, out GroundTile targetTile))
            return;

        if (targetTile.Type == GroundTile.EType.Wall || targetTile.isOpened)
            return;

        targetTile.Open(previousTile);
        targetTile.costSoFar = previousTile.costSoFar + Map.GetTileCost(targetTile.Type);
        targetTile.heuristic = GetHeuristic(mMap.GoalTile, targetTile);

        float priority = targetTile.costSoFar + targetTile.heuristic;
        openedTilePriorityQueue.Enqueue(targetTile, priority);
    }

    private float GetHeuristic(GroundTile goal, GroundTile target)
    {
        int h = Mathf.Abs(goal.Location.x - target.Location.x) + Mathf.Abs(goal.Location.y - target.Location.y);

        return h * (1f + HEURISTIC_ADJUST);
    }

    private void DrawPath(GroundTile goalTile)
    {
        // 찾아낸 경로 표시
        GroundTile previousTile = goalTile.PreviousTile;

        while (previousTile != null)
        {
            previousTile.SetColor(GroundTile.EType.Goal);

            previousTile = previousTile.PreviousTile;
        }
    }
}
