using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEngine;
using static GroundTile;

public class MapGenerator : MonoBehaviour
{
    public GroundTile prefGroundTile;
    public int width;
    public int height;

    public Map Generate(int _width, int _height)
    {
        var newMap = new Map(_width, _height);

        newMap.LoopMap((groundTile, x, y) =>
        {
            newMap.GroundTiles[x, y] = Instantiate<GroundTile>(prefGroundTile, new Vector3(x, 0, y), Quaternion.identity);

            var location = new Vector2Int(x, y);
            newMap.GroundTiles[x, y].Init(location);
        });

        return newMap;
    }

    public Map Generate()
    {
        return Generate(width, height);
    }
}
