
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Board board;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    private GameObject[,] pieces;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    public float RotateSpeed;

    private GameObject whiteKingInstantiated;
    private GameObject blackKingInstantiated;

    private List<string> moves;

    [HideInInspector]
    public GameObject lastMovedPiece;
    [HideInInspector]
    public Vector2Int lastPieceStartedFrom;

    private GameObject pawnUgradePiece;
    public GameObject pawnUpgradeUI;
    private Vector2Int pawnUpgradeGridPoint;

    void Awake()
    {
        instance = this;
        board = gameObject.GetComponent<Board>();
        moves = new List<string>();
    }

    void Start()
    {
        pieces = new GameObject[8, 8];

        white = new Player("white", true);
        black = new Player("black", false);

        currentPlayer = white;
        otherPlayer = black;

        InitialSetup();
        whiteKingInstantiated = pieces[4, 0];
        blackKingInstantiated = pieces[4, 7];

        //lastMovedPiece = null;
        
    }

    private void InitialSetup()
    {
        AddPiece(whiteRook, white, 0, 0);
        AddPiece(whiteKnight, white, 1, 0);
        AddPiece(whiteBishop, white, 2, 0);
        AddPiece(whiteQueen, white, 3, 0);
        AddPiece(whiteKing, white, 4, 0);
        AddPiece(whiteBishop, white, 5, 0);
        AddPiece(whiteKnight, white, 6, 0);
        AddPiece(whiteRook, white, 7, 0);

        for (int col = 0; col < 8; col++)
        {
            AddPiece(whitePawn, white, col, 1);
        }

        AddPiece(blackRook, black, 0, 7);
        AddPiece(blackKnight, black, 1, 7);
        AddPiece(blackBishop, black, 2, 7);
        AddPiece(blackQueen, black, 3, 7);
        AddPiece(blackKing, black, 4, 7);
        AddPiece(blackBishop, black, 5, 7);
        AddPiece(blackKnight, black, 6, 7);
        AddPiece(blackRook, black, 7, 7);

        for (int col = 0; col < 8; col++)
        {
            AddPiece(blackPawn, black, col, 6);
        }
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint)
    {
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
            board.SelectPiece(selectedPiece);
    }

    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        board.DeselctPiece(piece);    
    }

    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if(gridPoint.x < 0 || gridPoint.x >7 ||  gridPoint.y < 0 || gridPoint.y > 7)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }

    public Vector2Int GridForPiece(GameObject piece)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                    return new Vector2Int(i, j);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);
        if (piece == null)
            return false;
        return currentPlayer.pieces.Contains(piece);
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        return currentPlayer.pieces.Contains(piece);
    }

    public void Move(GameObject piece, Vector2Int gridPoint)
    {

        Vector2Int startGridPoint = GridForPiece(piece);
        // Remembering this piece's movement for the next player to check for Pawn en passe
        lastMovedPiece = piece;
        lastPieceStartedFrom = startGridPoint;
        // Actually moving the piece
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GameManager.instance.GridForPiece(pieceObject);

        var locations = piece.MoveLocations(gridPoint);

        //remove the off limit tiles
        locations.RemoveAll(tile => tile.x < 0 || tile.x > 7
            || tile.y < 0 || tile.y > 7);

        //remove the tiles containing friendly pieces
        locations.RemoveAll(tile => FriendlyPieceAt(tile));

        return locations;
    }

    public void NextPlayer()
    {
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;

        // FIXME Perform A Nicer Cmaera Transition
        //  Camera.main.transform.RotateAround(new Vector3(3.5f, 0f, 3.5f), this.gameObject.transform.up, 180f);

    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject capturedPiece = GameManager.instance.PieceAtGrid(gridPoint);
        currentPlayer.capturedPieces.Add(capturedPiece);
        
        pieces[gridPoint.x, gridPoint.y] = null;

        if(capturedPiece.GetComponent<Piece>().type == PieceType.King)
        {
            Debug.Log(currentPlayer.name + " wins");

            // Destry the tile selector and move slector components (objects) to end(read destroy) the state machine
            Destroy(gameObject.GetComponent<TileSelector>());
            Destroy(gameObject.GetComponent<MoveSelector>());
        }

        Destroy(capturedPiece);
    }

    public void AddMove(GameObject movingPiece,  Vector2Int gridPoint, bool capture)
    {
        Debug.Log(gridPoint);
        PieceType type = movingPiece.GetComponent<Piece>().type;    // TO FIX ::  Has some referrence to null problems with some pawn moves.

       
        string move = string.Empty;
        char col = (char)( (int)'a' + gridPoint.x);
        string row = (gridPoint.y + 1).ToString();
        if(type == PieceType.King)
        {
            move += "K";
        }
        else if(type == PieceType.Queen)
        {
            move += "Q";
        }
        else if(type == PieceType.Bishop)
        {
            move += "B";
        }
        else if(type == PieceType.Knight)
        {
            move += "N";
        }
        else if(type == PieceType.Rook)
        {
            move += "R";
        }
        else if(type == PieceType.Pawn && capture)
        {
            char previousCol = (char)((int)'a' + GameManager.instance.GridForPiece(movingPiece).x);
            move += previousCol.ToString();
        }
        if (capture)    // Adding that x to the move, if it was a capture
        {
            move += 'x'.ToString();
        }

        move += col.ToString()+row;
      

        // TODO Reference Point 1 : Add functionality to depict castling as well using O-O and o-o-o notations

        // Adding the move to the list of moves
        moves.Add(move);
        Debug.Log(move);
        
    }

    public bool IsKingInCheck(Player player)
    {
       GameObject king;
       if(player == white)
        {
            king = whiteKingInstantiated;
            // get the actual instantiated 
        }
        else
        {
            king = blackKingInstantiated;
        }

        King piece = king.GetComponent<King>();
        var underCheck =  piece.IsUnderCheck(GridForPiece(king), player);
        if(underCheck)
        {
            Debug.Log(player.name + " is under check");
            board.UnderAttack(king);
        }
        else
        {
            board.DeselctPiece(king);
        }

        return underCheck;
    }

    public void Castling(Vector2Int KingPosition,  bool left)
    {
  
        if(left)
        {
            Move(pieces[0, KingPosition.y], new Vector2Int(3, KingPosition.y));
            Move(pieces[4, KingPosition.y], new Vector2Int(2, KingPosition.y));
        }
        else
            
        {
            Move(pieces[7, KingPosition.y], new Vector2Int(5, KingPosition.y));
            Move(pieces[4, KingPosition.y], new Vector2Int(6, KingPosition.y));
        }
        return;
    }

    public void HandlePawnUpgradation(Vector2Int gridPoint)
    {
        //Destroy the current pawn Gameobject
        Destroy(PieceAtGrid(gridPoint));
        // Freezes the game
        Time.timeScale = 0f;
        pawnUgradePiece = null;
        pawnUpgradeGridPoint = gridPoint;
        pawnUpgradeUI.SetActive(true);

        // TODO :  PHandle instantiationg the selected game object at the desired gridpoint
    }
   
    public void QueenUpgrade()
    {
        if(otherPlayer.name == "white")
        {
            pawnUgradePiece = whiteQueen;
        }
        else
        {
            pawnUgradePiece = blackQueen;
        }
    }
    public void RookUpgrade()
    {
        if (otherPlayer.name == "white")
        {
            pawnUgradePiece = whiteRook;
        }
        else
        {
            pawnUgradePiece = blackRook;
        }
    }
    public void BishopUpgrade()
    {
        if (otherPlayer.name == "white")
        {
            pawnUgradePiece = whiteBishop;
        }
        else
        {
            pawnUgradePiece = blackBishop;
        }
    }
    public void KnightUpgrade()
    {
        if (otherPlayer.name == "white")
        {
            pawnUgradePiece = whiteKnight;
        }
        else
        {
            pawnUgradePiece = blackKnight;
        }
    }

    public void SelectionDone()
    {
        //Debug.Log(currentPlayer.name);
        if (pawnUgradePiece != null)
        {
            
            pawnUpgradeUI.SetActive(false);
            //TODO:   Complete Upgradation by going back to the function and instantiate required chess piece
            AddPiece(pawnUgradePiece, otherPlayer, pawnUpgradeGridPoint.x, pawnUpgradeGridPoint.y);
            Time.timeScale = 1f;
        }
        return;
    }

 

}

// TODO:
//GAME LOGIC
//1. IMPLEMENT CASTLING ON BOTH SIDES --- DONE ---
//2. IMPLEMENT PAWN UPGRADATION TO BASICALLY ANYTHING OTHER THAN A PAWN OR A KING
//3. IMPELEMENT PAWN ENPASSANT  --- DONE ---
//4. IMPLEMEMT CHESS AI so as to accomodate a PLAYER VS CPU style
//5. DETECT IF THE KING IS IN CHECK     --- DONE ---

//CAMERA
//1.  IMPLEMENT A SMOTHER TRANSITION OF CHANGING THE CAMERA TO BOTH SIDES

//PIECES
//1. IMPLEMENT A SMOTHER MOTION OF PIECES WHEN THEY MOVE TO THEIR RESPECTIVE PLACES

//SOUNDS:
//1. ADD SATISFYING SOUND EFFECTS FOR EVENTS LIKE PIECE SELECTION, PIECE MOVEMENT, PIECE CAPTURING ETC.,