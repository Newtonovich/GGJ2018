using System.Collections.Generic;
using UnityEngine;

public class InfluenceRadius : MonoBehaviour
{
    [SerializeField] private Vector3 initScale = new Vector3(8, 8, 8);
    [SerializeField] private Color goodZone;
    [SerializeField] private Color badZone;

    public int ID { get; private set; }

    public bool Dragging { get; private set; }
    private List<GameObject> collidedHouses;
    private List<GameObject> newCols;
    private int radius;
    private SpriteRenderer sRenderer;
    private bool canPosition;

    void OnEnable()
    {
        Dragging = true;
        collidedHouses = new List<GameObject>();
        transform.localScale = initScale;
        radius = Mathf.FloorToInt( transform.localScale.x / 2);
        sRenderer = GetComponent<SpriteRenderer>();
        sRenderer.color = badZone;
        canPosition = false;
    }

    public void MouseUp(int id)
    {
        if (canPosition)
        {
            Dragging = false;
            ID = id;
            GameManager.Instance.InfluenceAdded(this);
        }
        else
        {
            Dragging = false;
            Destroy(this.gameObject);
        }
    }

    private void ConnectHouses()
    {
        for (int i = 0; i < collidedHouses.Count; i++)
        {
            collidedHouses[i].GetComponent<IHouseController>().SetConnected();
        }
    }

    void Update()
    {
        if(Dragging)
        {
            if (Snap(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (!TooCloseToStation())
                {
                    Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, GameManager.Instance.HousesLayerMask);
                    newCols = new List<GameObject>();
                    for (int i = 0; i < cols.Length; i++)
                    {
                        newCols.Add(cols[i].gameObject);
                    }

                    CheckCollided(newCols);
                }
            }

        }
    }

    private bool Snap(Vector3 worldMousePos)
    {
        int x = Mathf.RoundToInt(worldMousePos.x);
        int y = Mathf.RoundToInt(worldMousePos.y);

        if (x == transform.position.x && y == transform.position.y)
            return false;

        transform.position = new Vector3(x, y, 0);
        return true;
    }

    private bool TooCloseToStation()
    {
        Vector3Int posInt = Vector3Int.RoundToInt(transform.position);

        if((posInt.x * posInt.x + posInt.y * posInt.y) <= 20)
        {
            sRenderer.color = badZone;
            canPosition = false;
            return true;
        }

        sRenderer.color = goodZone;
        canPosition = true;
        return false;
    }

    /*public void EnlargeScale()
    {
        this.transform.localScale += enlargeScale;
        radius = Mathf.FloorToInt(transform.localScale.x / 2);
    }*/

    private void CheckCollided(List<GameObject> newColArray)
    {
        List<GameObject> listCopy = new List<GameObject>(newColArray);
        List<GameObject> commonCols = new List<GameObject>();

        for (int i = 0; i < newColArray.Count; i++)
        {
            if (collidedHouses.Contains(newColArray[i]))
            {
                commonCols.Add(newColArray[i]);
                listCopy.Remove(newColArray[i]);
                collidedHouses.Remove(newColArray[i]);
            }
        }

        // What remains in the collided houses is the old houses
        for (int i = 0; i < collidedHouses.Count; i++)
        {
            IHouseController hc = collidedHouses[i].GetComponent<IHouseController>();
            if (hc != null)
            {
                hc.HouseOutOfRegion();
            }
        }

        // What remains in the listCopy are the new houeses
        for (int i = 0; i < listCopy.Count; i++)
        {
            IHouseController hc = listCopy[i].GetComponent<IHouseController>();
            if (hc != null)
            {
                hc.HouseInTempRegion();
            }
        }

        collidedHouses = new List<GameObject>();
        collidedHouses.AddRange(commonCols);
        collidedHouses.AddRange(listCopy);

    }

    public List<Vector3Int> GetInfluencedLocations()
    {
        List<Vector3Int> result = new List<Vector3Int>();

        int minY = Mathf.FloorToInt(transform.position.y - radius);
        int maxY = Mathf.FloorToInt(transform.position.y + radius);
        int minX = Mathf.FloorToInt(transform.position.x - radius);
        int maxX = Mathf.FloorToInt(transform.position.x + radius);

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j < maxX; j++)
            {
                int yPoint = Mathf.FloorToInt(i - transform.position.y);
                int xPoint = Mathf.FloorToInt(j - transform.position.x);

                if (((xPoint * xPoint) + (yPoint * yPoint)) <= radius * radius)
                {
                    Vector3Int a = new Vector3Int(j, i, 0);
                    result.Add(a);
                }
            }
        }

        return result;
    }

    private void PrintList<T>(string listName, List<T> listToPrint)
    {
        if (listToPrint == null || listToPrint.Count == 0)
            return;

        string result = "List " + listName + ":\n";
        for (int i = 0; i < listToPrint.Count; i++)
        {
            if(typeof(T) == typeof(GameObject))
                result += (listToPrint[i] as GameObject).name + ", ";
            else
                result += listToPrint[i].ToString() + ", ";
        }
        print(result);
    }
}
