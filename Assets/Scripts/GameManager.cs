using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject powerStation;
    [SerializeField] private GameObject influence;
    [SerializeField] private Transform housesParent;
    [SerializeField] private Transform connectorsParent;
    [SerializeField] private LayerMask housesLayer;
    [SerializeField] private float newHoodTimer;
    [SerializeField] private float timeToGetAngry;
    [SerializeField] private float newStationCounter;
    [SerializeField] private Timer timer;
    [SerializeField] private HoodLocationsMock hoodsLocationsMock;

    public Transform HousesParent { get { return housesParent; } }
    public Transform ConnectorsParent { get { return connectorsParent; } }
    public LayerMask HousesLayerMask { get { return housesLayer; } }
    public int InfluencesCounter { get { return influencesCounter; } }
    public float TimeToGetAngry { get { return timeToGetAngry; } }
    public int AvailableStation { get { return availableStation; } }
    public float NewStationCounter { get { return newStationCounter; } }
    public int MaxStrikes { get { return maxStrikes; } }
    public bool GameIsOver { get; private set; }
    public GameOverData CurrentGameOverData;

    // Delegates & Events
    public delegate void ScoreChangedEvent(int newScore);
    public event ScoreChangedEvent ScoreChanged;

    public delegate void StrikeChangedEvent(int newStrikeCount);
    public event StrikeChangedEvent StrikeChanged;

    public delegate void AvailableStationsChangedEvent(int newStationCount);
    public event AvailableStationsChangedEvent AvailableStationsChanged;

    private List<InfluenceRadius> influences;

    public List<Vector3Int> ForbiddenPositions { get { return forbiddenPositions; } }
    private List<Vector3Int> forbiddenPositions;
    private int influencesCounter;
    private float stationCountdown;
    private IHoodManager hoodManager;
    private int hoodToCreate;

    private int availableStation;
    private int points;
    private int maxStrikes = 15;
    private int strikes;

    private int highscore;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        NewGame();   
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void NewGame()
    {
        // Clears Previous houses and connectors
        if(housesParent.childCount > 0)
        {
            foreach (Transform child in housesParent)
            {
                Destroy(child.gameObject);
            }
        }

        if (connectorsParent.childCount > 0)
        {
            foreach (Transform child in connectorsParent)
            {
                Destroy(child.gameObject);
            }
        }

        EventManager.TriggerEvent(ConstantNames.ShowOpeningPanel);

        if(hoodManager == null)
            hoodManager = GetComponent<IHoodManager>();

        hoodManager.NewGame();
        hoodToCreate = 0;
        points = 0;
        strikes = 0;
        ScoreChanged(points);
        StrikeChanged(strikes);

        availableStation = 2;

        highscore = PlayerPrefs.HasKey(ConstantNames.Highscore) ? PlayerPrefs.GetInt(ConstantNames.Highscore) : 0;
        GameIsOver = false;
    }

    public void SetGameWindows()
    {
        EventManager.TriggerEvent(ConstantNames.ShowInGamePanel);
    }

    public void StartGame()
    {
        GameIsOver = false;

        UpdateStationCounter();
        Instantiate(powerStation, Vector3Int.zero, Quaternion.identity);
        forbiddenPositions = new List<Vector3Int>();
        influences = new List<InfluenceRadius>();
        influencesCounter = 0;
        PopulateForbidden();

        //StartCoroutine(RefillStationCount());
        timer.Set(newStationCounter, RefillStationCount);
        StartCoroutine(CreateHoods());
    }

    IEnumerator CreateHoods()
    {
        while (true)
        {
            if (hoodToCreate >= hoodsLocationsMock.HoodsPoses.Count)
                break;

            while (hoodManager.Houses.Contains(hoodsLocationsMock.HoodsPoses[hoodToCreate]))
            {
                hoodToCreate++;
            }

            hoodManager.StartNewHood(hoodsLocationsMock.HoodsPoses[hoodToCreate]);
            hoodToCreate++;
            yield return new WaitForSeconds(newHoodTimer);
        }
    }

    private void PopulateForbidden()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                forbiddenPositions.Add(new Vector3Int(j, i, 0));
            }
        }

        for (int j = -1; j < 2; j++)
        {
            forbiddenPositions.Add(new Vector3Int(j, -2, 0));
            forbiddenPositions.Add(new Vector3Int(j, 2, 0));
        }
    }

    public void InfluenceAdded(InfluenceRadius ir)
    {
        influences.Add(ir);
        influencesCounter++;
        availableStation--;
        UpdateStationCounter();
        EventManager.TriggerEvent(ConstantNames.InfluencePlaced);

    }

    public bool InInfluenceArea(Vector3Int point)
    {
        for (int i = 0; i < influences.Count; i++)
        {
            if (influences[i].GetInfluencedLocations().Contains(point))
                return true;
         }

        return false;
    }

    private void UpdateStationCounter()
    {
        AvailableStationsChanged(availableStation);
    }

    private void RefillStationCount()
    {
        availableStation++;
        UpdateStationCounter();            
    }

    public void HousePositioned()
    {
        points++;
        ScoreChanged(points);
    }

    public void HouseGotAngry()
    {
        strikes++;
        StrikeChanged(strikes);

        if(strikes >= maxStrikes)
        {
            //Debug.Log("Game Over");
            GameOver();
        }
    }

    public void GameOver()
    {
        bool isNewHigh = false;
        GameIsOver = true;

        if(points > highscore)
        {
            isNewHigh = true;
            highscore = points;
            PlayerPrefs.SetInt(ConstantNames.Highscore, highscore);
        }

        CurrentGameOverData = new GameOverData(points, highscore, isNewHigh);

        EventManager.TriggerEvent(ConstantNames.ShowGameOverPanel);
    }
}
