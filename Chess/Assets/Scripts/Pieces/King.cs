using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool HasMoved;
    private Vector2Int[] KingDirections = { new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1),
            new Vector2Int(1,0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)};

    public King()   // constructor
    {
        HasMoved = false;
        type = PieceType.King;
    }

    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        
        foreach(var direction in KingDirections)
        {
            Vector2Int newLocation = new Vector2Int(gridPoint.x + direction.x, gridPoint.y + direction.y);
            if(newLocation.x < 0 || newLocation.x > 7 || newLocation.y < 0 || newLocation.y > 7 || GameManager.instance.FriendlyPieceAt(newLocation))
            {
                continue;
            }
            locations.Add(newLocation);
        }

        // Reference point : Add castling feature
        if(!HasMoved)   // King hasnt moved
        {
            // Check for rook on column 0 and on same row as this king so [0, gridPoint.y] where that can be o or 7
            GameObject leftRook = GameManager.instance.PieceAtGrid(new Vector2Int(0, gridPoint.y));
            if(leftRook != null && leftRook.GetComponent<Piece>().type == PieceType.Rook)
            {
                Rook rook = leftRook.GetComponent<Rook>();
                if( rook.HasMoved == false)  // leftRook hasn't moved
                {
                    // Check if all the spots between them are empty
                    int i = 1;
                    while( i < 4)
                    {
                        if (GameManager.instance.PieceAtGrid(new Vector2Int(i, gridPoint.y)) != null)
                            break;
                        i++;
                    }
                    if(i == 4)  // add the left castling move to locations
                    {
                        locations.Add(new Vector2Int(2, gridPoint.y));
                    }
                }
            }

            // TODO : Write a function that does not duplicate this code

            // Do the same thing if rook is present on the last column and same row as our King
            GameObject rightRook = GameManager.instance.PieceAtGrid(new Vector2Int(7, gridPoint.y));
            if(rightRook != null && rightRook.GetComponent<Piece>().type == PieceType.Rook)
            {
                Rook rook = rightRook.GetComponent<Rook>();
                if( rook.HasMoved == false)
                {
                    int i = 6;
                    while( i < 7 )
                    {
                        if (GameManager.instance.PieceAtGrid(new Vector2Int(i, gridPoint.y)) != null)
                            break;
                        i++;
                    }
                    if( i == 7)   // add the right castling move to locations
                    {
                        locations.Add(new Vector2Int(6, gridPoint.y));
                    }
                }
            }
        }


        return locations;
    }

    public bool IsUnderCheck(Vector2Int gridPoint, Player player)
    {

        foreach (var direction in RookDirections)
        {
            for(int i = 1; i < 8; i++)
            {
                Vector2Int newLocation = new Vector2Int(gridPoint.x + direction.x * i, gridPoint.y + direction.y * i);
                GameObject piece = GameManager.instance.PieceAtGrid(newLocation);

                if (newLocation.x < 0 || newLocation.x > 7 || newLocation.y < 0 || newLocation.y > 7 || player.pieces.Contains(piece))
                {
                    break;
                }
                PieceType type;
                if(piece != null)
                {
                    type = piece.GetComponent<Piece>().type;
                    if(type == PieceType.Queen || type == PieceType.Rook)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }
        // check in all 8 directions
        foreach (var direction in BishopDirections)
        {
            for(int i = 1; i < 8; i++)
            {
                Vector2Int newLocation = new Vector2Int(gridPoint.x + direction.x, gridPoint.y + direction.y);
                GameObject piece = GameManager.instance.PieceAtGrid(newLocation);

                if (newLocation.x < 0 || newLocation.x > 7 || newLocation.y < 0 || newLocation.y > 7 || player.pieces.Contains(piece))
                {
                    break;
                }
                PieceType type;
                if (piece != null)
                {
                    type = piece.GetComponent<Piece>().type;
                    if (type == PieceType.Queen || type == PieceType.Bishop)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        // detecting enemy pawns
        Vector2Int forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + player.forward);
        Vector2Int forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + player.forward);
        GameObject opponentPiece = GameManager.instance.PieceAtGrid(forwardRight);
        if(opponentPiece != null && !player.pieces.Contains(opponentPiece) && opponentPiece.GetComponent<Piece>().type == PieceType.Pawn)
        {
            return true;
        }

        opponentPiece = GameManager.instance.PieceAtGrid(forwardLeft);
        if (opponentPiece != null && !player.pieces.Contains(opponentPiece) && opponentPiece.GetComponent<Piece>().type == PieceType.Pawn)
        {
            return true;
        }


        // check for attacks by opponents Horse (Knight)
        foreach (var direction in KnightDirections)
        {
            Vector2Int newLocation = new Vector2Int(gridPoint.x + direction.x, gridPoint.y + direction.y);
            GameObject piece = GameManager.instance.PieceAtGrid(newLocation);
            if (newLocation.x < 0 || newLocation.x > 7 || newLocation.y < 0 || newLocation.y > 7 || player.pieces.Contains(piece))
            {
                continue;
            }
            //GameObject piece = GameManager.instance.PieceAtGrid(newLocation);
            if (piece != null && piece.GetComponent<Piece>().type == PieceType.Knight)
            {
                return true;
            }
        }

        return false;
    }


    
}
