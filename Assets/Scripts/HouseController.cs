using System.Collections;
using UnityEngine;

public class HouseController : MonoBehaviour, IHouseController
{
    [SerializeField] private Renderer houseRenderer;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private GameObject xmark;

    public bool HasElectricity { get { return houseRenderer.material.color != Color.green; } }
    public bool IsVisible { get { return houseRenderer.isVisible; } }
    public Bounds SpriteBounds { get { return houseRenderer.bounds; } }

    private Coroutine GetAngryCor;
    private float maxTime;
    private float counter = 0;
    private bool beyondRepair = false;
    private bool Connected = false;

    void Awake()
    {
        //houseRenderer = GetComponent<Renderer>();
        maxTime = GameManager.Instance.TimeToGetAngry;
        GetAngryCor = StartCoroutine(GettingAngry());
        xmark.SetActive(false);
        HouseOutOfRegion();
    }

    public void HouseInTempRegion()
    {
        if (Connected || beyondRepair)
            return;

        checkmark.SetActive(true);
    }

    public void HouseOutOfRegion()
    {
        if (Connected)
            return;

        checkmark.SetActive(false);
    }

    public void PaintHouse()
    {
        if (beyondRepair)
            return;

        StartCoroutine(WaitFrameToPaintHouse());
    }

    private IEnumerator WaitFrameToPaintHouse()
    {
        while(houseRenderer == null)
        {
            yield return null;
        }

        if (GetAngryCor != null)
        {
            StopCoroutine(GetAngryCor);
        }

        houseRenderer.material.color = Color.green;
    }

    public void ClearHouse()
    {
        //print(this.gameObject.name + ": " + Connected + "; " + beyondRepair);
        
        if(GameManager.Instance.InInfluenceArea(PositionInt()))
        {
            Connected = true;
        }

        if (Connected || beyondRepair)
            return;

        houseRenderer.material.color = Color.white;
        if (GetAngryCor != null)
        {
            StopCoroutine(GetAngryCor);
        }
        GetAngryCor = StartCoroutine(GettingAngry());
    }

    private void CheckNewInfluenceArea()
    {
        Vector3Int pos = Vector3Int.RoundToInt(transform.position);
        if (GameManager.Instance.InInfluenceArea(pos))
        {
            SetConnected();
            if (GetAngryCor != null)
            {
                StopCoroutine(GetAngryCor);
            }
        }
        
    }

    private IEnumerator GettingAngry()
    {
        EventManager.StartListening(ConstantNames.InfluencePlaced, CheckNewInfluenceArea);
        counter = 0;
        while(counter < maxTime && !GameManager.Instance.GameIsOver)
        {
            yield return null;
            houseRenderer.material.color = Color.Lerp(Color.white, Color.red, counter / maxTime);
            counter += Time.deltaTime;
        }

        if (GameManager.Instance.GameIsOver)
            yield break;

        GameManager.Instance.HouseGotAngry();
        xmark.SetActive(true);
        beyondRepair = true;
        EventManager.StopListening(ConstantNames.InfluencePlaced, CheckNewInfluenceArea);
    }

    public void SetConnected()
    {
        if (Connected)
            return;

        checkmark.SetActive(false);
        GameManager.Instance.HousePositioned();

        Connected = true;
        PaintHouse();
    }

    public Vector3Int PositionInt()
    {
        return Vector3Int.RoundToInt(transform.position);
    }

}
