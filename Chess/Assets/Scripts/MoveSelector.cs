using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveSelector : MonoBehaviour
{
    public GameObject attackLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject moveLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;

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
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Vector2Int gridPoint = Geometry.GridFromPoint(hit.point);
            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

            if(Input.GetMouseButtonDown(0))
            {
                // Referenceoint 2:  Call a function here to validate the move
                if(GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    GameManager.instance.Move(movingPiece, gridPoint);
                }

                // Reference point 3: Handle Capturing an enemy piece at the grid point if any

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
        Debug.Log("in enter state move piece;");
    }

    private void ExitState()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();
    }
}
