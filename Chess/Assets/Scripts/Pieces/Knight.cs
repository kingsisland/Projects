
using System.Collections.Generic;

using UnityEngine;

public class Knight : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        foreach(var direction in KnightDirections)
        {
            Vector2Int newLocation = new Vector2Int(gridPoint.x + direction.x, gridPoint.y + direction.y);
            locations.Add(newLocation);
        }
        return locations;
    }
}
