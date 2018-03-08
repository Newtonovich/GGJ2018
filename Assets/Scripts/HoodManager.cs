using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodManager : MonoBehaviour, IHoodManager
{
    [SerializeField] private GameObject houseObject;

    private int index;
    private List<Hood> hoods;
    private Coroutine expendingCor;
    private int currentHood;
    private int maxAmountOfHoods;

    public List<Vector3Int> Houses
    {
        get
        {
            List<Vector3Int> result = new List<Vector3Int>();
            if (hoods == null)
                return result;

            for (int i = 0; i < hoods.Count; i++)
            {
                result.AddRange(hoods[i].Houses);
            }
            return result;
        }
    }

    public void NewGame()
    {
        hoods = new List<Hood>();
        index = 0;
        currentHood = 0;
        maxAmountOfHoods = 0;
    }

    public void StartNewHood(Vector3Int newCenter)
    {
        Hood hood = new Hood(newCenter);
        hoods.Add(hood);
        maxAmountOfHoods = Mathf.Max(maxAmountOfHoods, hoods.Count);

        GameObject hGO = Instantiate(houseObject, newCenter, Quaternion.identity, GameManager.Instance.HousesParent);
        hGO.name = hGO.name.Replace("Clone", index.ToString());
        IHouseController hc = hGO.GetComponent<IHouseController>();

        if (GameManager.Instance.InInfluenceArea(newCenter))
        {
            hc.PaintHouse();
            hc.SetConnected();
        }

        StartCoroutine(WaitToCheckVisibility(hc));

        if (expendingCor == null)
        {
            expendingCor = StartCoroutine(Expend());
        }
    }

    public void CreateNewHouse()
    {
        Vector3Int newHousePos;

        currentHood = currentHood % hoods.Count;

        do
        {
            newHousePos = hoods[currentHood].GetFreeAdjacentSpace();
        }
        while (GameManager.Instance.ForbiddenPositions.Contains(newHousePos));

        // Check if collide with other hoods. If so, merge these two hoods
        for (int i = 0; i < hoods.Count; i++)
        {
            if(hoods[i].Houses.Contains(newHousePos))
            {
                if(i == currentHood)
                {
                    Debug.LogErrorFormat("What? both hood {0} & {1} contains house location {2}", i, currentHood, newHousePos);
                    break;
                }
                hoods[i].MergeHood(hoods[currentHood]);
                hoods.RemoveAt(currentHood);
                return;
            }
        }

        index++;
        hoods[currentHood].PositionHouse(newHousePos);

        GameObject hGO = Instantiate(houseObject, newHousePos, Quaternion.identity, GameManager.Instance.HousesParent);
        hGO.name = hGO.name.Replace("Clone", index.ToString());
        IHouseController hc = hGO.GetComponent<IHouseController>();

        if (GameManager.Instance.InInfluenceArea(newHousePos))
        {
            hc.PaintHouse();
            hc.SetConnected();

        }

        StartCoroutine(WaitToCheckVisibility(hc));

        currentHood++;
        currentHood = currentHood % hoods.Count;
    }

    private IEnumerator Expend()
    {
        while (!GameManager.Instance.GameIsOver)
        {
            yield return new WaitForSeconds(2f / maxAmountOfHoods + 0.75f);
            CreateNewHouse();
        }
    }

    private IEnumerator WaitToCheckVisibility(IHouseController hc)
    {
        yield return null;
        Vector3Int hcPos = hc.PositionInt();
        float screenRatio = Screen.width / Screen.height;
        float camSize = Camera.main.orthographicSize;
        int distanceToEnlarge = 0;

        // Vertical distance
        if (hcPos.y > camSize)
        {
            int floor = Mathf.FloorToInt(camSize);
            distanceToEnlarge = hcPos.y - floor;
        }

        // Horizontal Distance
        else if(hcPos.x > camSize * screenRatio)
        {
            int floor = Mathf.FloorToInt(camSize * screenRatio);
            distanceToEnlarge = hcPos.x - floor;
            distanceToEnlarge++; // Just in case
        }

        if(distanceToEnlarge > 0)
        {
            float counter = 0;
            float targetCamSize = camSize + distanceToEnlarge;
            while (Camera.main.orthographicSize < targetCamSize)
            {
                Camera.main.orthographicSize = Mathf.Lerp(camSize, targetCamSize, counter / 0.7f);
                counter += Time.deltaTime;
                yield return null;
            }
        }
    }
}
