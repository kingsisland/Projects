using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public bool HasMoved { get; set; }
    public Rook ()
    {
        HasMoved = false;
        type = PieceType.Rook;
    }

    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        foreach(var direction in RookDirections)
        {
            for(int i =1; i < 8; i++)
            {
                Vector2Int nextCell = new Vector2Int(gridPoint.x + direction.x * i, gridPoint.y + direction.y * i);
                if (nextCell.x < 0 || nextCell.x > 7 || nextCell.y < 0 || nextCell.y > 7)
                {
                    break;
                }
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
