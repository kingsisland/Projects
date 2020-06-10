
using System;
using System.Collections.Generic;
using UnityEngine;


public class MoveSelector : MonoBehaviour
{
    public GameObject attackLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject moveLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)),
                                        Quaternion.identity, gameObject.transform);

        tileHighlight.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2Int gridPoint = Geometry.GridFromPoint(hit.point);
            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

            if (Input.GetMouseButtonDown(0))
            {
               

                if(!moveLocations.Contains(gridPoint)) // Is not a valid move 
                {
                    //return;
                    Debug.Log("Moving to "+ gridPoint.ToString() +" is not logically possible");
                    ExitState(false);
                }
                else if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    //GameManager.instance.AddMove(movingPiece, gridPoint, false);

                    //  find out if moving piece is King and check if it just castled
                    if (movingPiece.GetComponent<Piece>().type == PieceType.King)
                    {
                        if(movingPiece.GetComponent<King>().HasMoved == false)
                        {
                            if (gridPoint.x == 2)
                            {
                                GameManager.instance.Castling(gridPoint, true);
                            }
                            else if(gridPoint.x == 6)
                            {
                                GameManager.instance.Castling(gridPoint, false);
                            }
                        }
                    }
                    else if(movingPiece.GetComponent<Piece>().type == PieceType.Pawn)
                    {
                        GameManager.instance.Move(movingPiece, gridPoint);
                        // Checking for Pawn  En passe 
                        if (Mathf.Abs(GameManager.instance.GridForPiece(movingPiece).y -
                        gridPoint.y) == 1 && Math.Abs(GameManager.instance.GridForPiece(movingPiece).x - gridPoint.x) == 1)
                        {
                            if (GameManager.instance.GridForPiece(movingPiece).x > gridPoint.x)
                            {
                                GameManager.instance.CapturePieceAt(new Vector2Int(gridPoint.x, gridPoint.y - GameManager.instance.currentPlayer.forward));
                            }
                            else
                            {
                                GameManager.instance.CapturePieceAt(new Vector2Int(gridPoint.x, gridPoint.y - GameManager.instance.currentPlayer.forward));
                            } 
                        }
                        HasPawnReachedTheOtherSide(gridPoint);
                        
                    }
                    else
                    {
                        GameManager.instance.Move(movingPiece, gridPoint);
                    }
                    
                }
                // Reference point 3: Handle Capturing an enemy piece at the grid point if any
                else
                {
                    //GameManager.instance.AddMove(movingPiece, gridPoint, true);
                    GameManager.instance.CapturePieceAt(gridPoint);
                    GameManager.instance.Move(movingPiece, gridPoint);
                    // Check for pawn upgradation
                    if (movingPiece.GetComponent<Piece>().type == PieceType.Pawn)
                    {
                        HasPawnReachedTheOtherSide(gridPoint);
                    }
                }


                if( GameManager.instance.IsKingInCheck(GameManager.instance.currentPlayer))    // Current Player's King is in check
                {
                    // Rewind the move OR Invalidate the current move
                }

                if(GameManager.instance.IsKingInCheck(GameManager.instance.otherPlayer))   // Opponents King is in check
                {
                    Debug.Log("King is in check");
          
                }
            
                ExitState(true);
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }


    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();

        foreach(Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if(GameManager.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            locationHighlights.Add(highlight);
        }
    }

    private void ExitState(bool nextPlayer)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
        if(nextPlayer)
        {
            GameManager.instance.NextPlayer();
        }

        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();
    }

    void HasPawnReachedTheOtherSide(Vector2Int gridPoint)
    {
        int forward = GameManager.instance.currentPlayer.forward;
        if ((gridPoint.y == 7 && forward == 1) || (gridPoint.y == 0 && forward == -1))
        {
            // TODO : REFERENCE POINT : call the gamemangaer to handle pawn upgradation
            GameManager.instance.HandlePawnUpgradation(gridPoint);
        }
        return;
    }
}
