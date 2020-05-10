using UnityEngine;


public class TileSelector : MonoBehaviour
{
   

    public GameObject titleHighlightPrefab;
    private GameObject titleHighlight;
    // Start is called before the first frame update
    void Start()
    {
        Vector2Int gridPoint = Geometry.GridPoint(0, 0);
        Vector3 point = Geometry.PointFromGrid(gridPoint);
        titleHighlight = Instantiate(titleHighlightPrefab, point, Quaternion.identity);
        titleHighlight.SetActive(false);




    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            titleHighlight.SetActive(true);
            titleHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
        }
        else
        {
            titleHighlight.SetActive(false);
        }
    }

    public void EnterState()
    {
        enabled = true;
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
