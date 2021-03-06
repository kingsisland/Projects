﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { King, Queen, Bishop, Rook, Knight, Pawn};

public abstract class Piece : MonoBehaviour
{
    public PieceType type;

    protected Vector2Int[] RookDirections = { new Vector2Int(1, 0), new Vector2Int(0, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1)};
    protected Vector2Int[] BishopDirections = { new Vector2Int(1, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, -1), new Vector2Int(1, -1)};
    protected Vector2Int[] KnightDirections = { new Vector2Int(-1, -2), new Vector2Int(-2, -1), new Vector2Int(-2, 1), new Vector2Int(-1, 2),
            new Vector2Int(1, 2), new Vector2Int(2, 1), new Vector2Int(2, -1), new Vector2Int(1, -2)};

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);
}
