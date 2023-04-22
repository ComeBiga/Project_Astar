using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public GroundTile[,] GroundTiles => mGroundTiles;
    public GroundTile StartTile => mStartTile;
    public GroundTile GoalTile => mGoalTile;

    public int Width => mGroundTiles.GetLength(0);
    public int Height => mGroundTiles.GetLength(1);

    private GroundTile[,] mGroundTiles;
    private GroundTile mStartTile;
    private GroundTile mGoalTile;

    private const int COST_WALL = 1000;
    private const int COST_GROUND = 1;

    public Map(GroundTile[,] groundTiles)
    {
        mGroundTiles = groundTiles;
    }

    public Map(int width, int height)
    {
        mGroundTiles = new GroundTile[width, height];
    }

    public void Clear()
    {
        LoopMap((groundTile, x, y) =>
        {
            groundTile.Reset();
        });
    }

    public bool TryGetGroundTile(int x, int y, out GroundTile resultTile)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            resultTile = null;
            return false;
        }

        resultTile = mGroundTiles[x, y];
        return true;
    }

    public void Set(GroundTile targetTile, GroundTile.EType type)
    {
        switch(targetTile.Type)
        {
            case GroundTile.EType.Start:
                mStartTile = null;
                break;
            case GroundTile.EType.Goal: 
                mGoalTile = null;
                break;
            default:
                break;
        }

        targetTile.Set(type);
    }

    public void SetStart(GroundTile targetTile)
    {
        mStartTile?.Set(GroundTile.EType.Ground);
        mStartTile = targetTile;

        mStartTile.Set(GroundTile.EType.Start);
    }

    public void SetGoal(GroundTile targetTile)
    {
        mGoalTile?.Set(GroundTile.EType.Ground);
        mGoalTile = targetTile;

        mGoalTile.Set(GroundTile.EType.Goal);
    }

    public void LoopMap(System.Action<GroundTile, int, int> action)
    {
        for (int x = 0; x < Width; ++x)
        {
            for (int y = 0; y < Height; ++y)
            {
                action.Invoke(mGroundTiles[x, y], x, y);
            }
        }
    }

    public static int GetTileCost(GroundTile.EType type)
    {
        switch (type)
        {
            case GroundTile.EType.Wall:
                return COST_WALL;
            default:
                return COST_GROUND;
        }
    }
}
