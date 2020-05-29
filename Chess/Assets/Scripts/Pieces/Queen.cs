using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        foreach(var directions in BishopDirections)
        {
            for(int i = 1; i < 8; i++)
            {
                Vector2Int newLocation = new Vector2Int(gridPoint.x + i * directions.x, gridPoint.y + i * directions.y);
                if(newLocation.x <0 || newLocation.x > 7 || newLocation.y < 0 || newLocation.y > 7)
                {
                    break;
                }
                locations.Add(newLocation);
                if(GameManager.instance.PieceAtGrid(newLocation))
                {
                    break;
                }
            }
        }
        foreach (var directions in RookDirections)
        {
            for (int i = 1; i < 8; i++)
            {
                Vector2Int newLocation = new Vector2Int(gridPoint.x + i * directions.x, gridPoint.y + i * directions.y);
                if (newLocation.x < 0 || newLocation.x > 7 || newLocation.y < 0 || newLocation.y > 7)
                {
                    break;
                }
                locations.Add(newLocation);
                if (GameManager.instance.PieceAtGrid(newLocation))
                {
                    break;
                }
            }
        }

        return locations;
    }
}
