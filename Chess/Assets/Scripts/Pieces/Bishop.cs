
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        foreach(Vector2Int direction in BishopDirections)
        {
            for(int i = 1; i < 8; i++)
            {
                Vector2Int nextCell = new Vector2Int(gridPoint.x + i * direction.x, gridPoint.y + i * direction.y);
                if (nextCell.x < 0 || nextCell.x > 7 || nextCell.y < 0 || nextCell.y > 7)
                    break;
                locations.Add(nextCell);
                if (GameManager.instance.PieceAtGrid(nextCell))
                {
                    break;
                }
            }
        }

        return locations;
    }
}
