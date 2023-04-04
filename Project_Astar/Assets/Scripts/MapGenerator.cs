using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;

    private int[,] mMap;

    private void Start()
    {
        mMap = new int[width, height];

        LoopMap((x, y) =>
        {
            mMap[x, y] = 0;
        });
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
                    Gizmos.color = (mMap[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
