
using System.Collections.Generic;

using UnityEngine;

public class Pawn : Piece
{
    public bool HasMoved = false;
    public bool didEnPasse = false;

    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();

        int forwardDirection = GameManager.instance.currentPlayer.forward;

        //  TODO : add mechanism to enable EN PASSE
       
        
        // check for left
        if(!didEnPasse)
        {
            GameObject leftPiece = GameManager.instance.PieceAtGrid(new Vector2Int(gridPoint.x - 1, gridPoint.y));
            if (leftPiece != null && leftPiece.GetComponent<Piece>().type == PieceType.Pawn &&
                GameManager.instance.lastMovedPiece == leftPiece && Mathf.Abs(GameManager.instance.lastPieceStartedFrom.y - gridPoint.y) == 2)
            {
                locations.Add(new Vector2Int(gridPoint.x -1, gridPoint.y + GameManager.instance.currentPlayer.forward));
            }
        }

        // check for right
        if(!didEnPasse)
        {
            GameObject rightPiece = GameManager.instance.PieceAtGrid(new Vector2Int(gridPoint.x + 1, gridPoint.y));
            if( rightPiece != null && rightPiece.GetComponent<Piece>().type == PieceType.Pawn &&
                GameManager.instance.lastMovedPiece == rightPiece && Mathf.Abs(GameManager.instance.lastPieceStartedFrom.y - gridPoint.y) == 2)
            {
                locations.Add(new Vector2Int(gridPoint.x + 1, gridPoint.y + GameManager.instance.currentPlayer.forward));
            }

        }


        // normal forward steps
        Vector2Int forward = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
        if(!GameManager.instance.PieceAtGrid(forward))
        {
            locations.Add(forward);     // one step forawrd

            Vector2Int doubleForward = new Vector2Int(gridPoint.x, gridPoint.y + (forwardDirection * 2));

            if (!HasMoved && !GameManager.instance.PieceAtGrid(doubleForward))  
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
