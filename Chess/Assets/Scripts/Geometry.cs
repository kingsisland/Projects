
using UnityEngine;

static public class Geometry
{
    static public Vector3 PointFromGrid(Vector2Int gridPoint)
    {
        float x = 1.0f * gridPoint.x;
        float z = 1.0f * gridPoint.y;
        return new Vector3(x, 0.015f, z);

    }

    static public Vector2Int GridPoint(int col, int row)
    {
        return new Vector2Int(col, row);
    }

    static public Vector2Int GridFromPoint(Vector3 point)
    {
        int col = Mathf.FloorToInt(0.5f + point.x);
        int row = Mathf.FloorToInt(0.5f + point.z);
        return new Vector2Int(col, row);
    }

}
