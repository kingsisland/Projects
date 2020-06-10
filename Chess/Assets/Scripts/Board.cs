
using UnityEngine;

public class Board : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectedMaterial;
    public Material UnderAttackMaterial;
    public GameObject AddPiece(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint),
                                                Quaternion.identity, gameObject.transform);
        //MeshRenderer renderer = newPiece.GetComponentInChildren<MeshRenderer>();
        //renderer.material = defaultMaterial;
        return newPiece;
    }

    public void RemovePiece(GameObject piece)
    {
        Destroy(piece);
    }

    public void MovePiece(GameObject piece, Vector2Int gridPoint)
    {
        Piece pieceScript = piece.GetComponent<Piece>();

        // Marking HasMoved to TRUE if piece is Rook or King, so as to stop them from castling
        if (pieceScript.type == PieceType.Rook)
        {
            Rook rook = piece.GetComponent<Rook>();
            rook.HasMoved = true;            
        }
        else if(pieceScript.type == PieceType.King)
        {
            King king = piece.GetComponent<King>();
            king.HasMoved = true;
        }
        else if (pieceScript.type == PieceType.Pawn)
        {
            Pawn pawn = piece.GetComponent<Pawn>();
            pawn.HasMoved = true;
        }

        piece.transform.position = Geometry.PointFromGrid(gridPoint);
    }

    public void SelectPiece(GameObject piece)
    {
        MeshRenderer renderer = piece.GetComponentInChildren<MeshRenderer>();
        renderer.material = selectedMaterial;
    }

    public void DeselctPiece(GameObject piece)
    {
        //Debug.Log(piece.GetComponent<Piece>().type);    // Logs the type of piece on chess board

        MeshRenderer renderer = piece.GetComponentInChildren<MeshRenderer>();
        renderer.material = defaultMaterial;
    }

    public void UnderAttack(GameObject piece)
    {
        MeshRenderer renderer = piece.GetComponentInChildren<MeshRenderer>();
        renderer.material = UnderAttackMaterial;
    }
}
