using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Pawn : Piece
{
    bool firstMove= true;

    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        int forwardDirection = GameManager.instance.currentPlayer.forward;
        
        //  To ADD : add mechanism to enable EN PASSE

        Vector2Int forward = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
        if(!GameManager.instance.PieceAtGrid(forward))
        {
            locations.Add(forward);     // one step forawrd

            Vector2Int doubleForward = new Vector2Int(gridPoint.x, gridPoint.y + (forwardDirection * 2));

            if (firstMove && !GameManager.instance.PieceAtGrid(doubleForward))  
            {
                locations.Add(doubleForward);
            }
        }
        Vector2Int forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + forwardDirection);
        Vector2Int forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + forwardDirection);

        if(!GameManager.instance.FriendlyPieceAt(forwardRight) && GameManager.instance.PieceAtGrid(forwardRight))   // capture right piece
        {
            locations.Add(forwardRight);
        }
        if(!GameManager.instance.FriendlyPieceAt(forwardLeft) && GameManager.instance.PieceAtGrid(forwardLeft))     // capture left piece
        {
            locations.Add(forwardLeft);
        }


        return locations;
    }
}
