using UnityEngine;
using UnityEngine.UIElements;

public class TileSelector : MonoBehaviour
{
   

    public GameObject tileHighlightPrefab;
    private GameObject tileHighlight;
    private GameObject selectedPiece;
    // Start is called before the first frame update
    void Start()
    {
        Vector2Int gridPoint = Geometry.GridPoint(0, 0);
        Vector3 point = Geometry.PointFromGrid(gridPoint);
        tileHighlight = Instantiate(tileHighlightPrefab, point, Quaternion.identity);
        tileHighlight.SetActive(false);
        selectedPiece = null;

    }

    // Update is called once per frame
    void Update()
    {
        // select a tileby hovering over it
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

            if (Input.GetMouseButtonDown(0))
            {
                if(selectedPiece != null)
                {
                    GameManager.instance.DeselectPiece(selectedPiece);
                }
                selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);
                if(GameManager.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
                {
                    GameManager.instance.SelectPiece(selectedPiece);
                    // Reference Point 1: add ExitState call here later

                    ExitState(selectedPiece);
                }
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }

        
    }

    public void EnterState()
    {
        enabled = true;
    }

    private void ExitState(GameObject movingPiece)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        MoveSelector move = GetComponent<MoveSelector>();
        move.EnterState(movingPiece);

    }
    #region 
    void InstantiateChessTiles()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Position pos = gameObject.transform.position;
        for (int i = 0; i < 8; i++)
        {

            for (int j = 0; j < 8; j++)
            {
                GameObject instantiated = Instantiate(cube, new Vector3(i - 6.5f, 0.65f, j - 0.5f), Quaternion.identity);
                if (i % 2 == 1 && j % 2 == 1 || i % 2 == 0 && j % 2 == 0)
                {
                    instantiated.GetComponent<Renderer>().material.color = Color.black;
                }
            }
        }
        Destroy(cube);
    }
    #endregion
}
