
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
                    return;
                }
                if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                // Reference point 3: Handle Capturing an enemy piece at the grid point if any
                else
                {
                    GameManager.instance.CapturePieceAt(gridPoint);
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                

                ExitState();
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

    private void ExitState()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
        GameManager.instance.NextPlayer();

        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();
    }
}
