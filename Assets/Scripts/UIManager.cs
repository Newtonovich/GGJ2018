using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text availableStationsText;
    [SerializeField] private Text timerText;

    [SerializeField] private Text pointsText;
    [SerializeField] private Text strikesText;
    [SerializeField] private Text outOfStrikesText;

    [SerializeField] private GameObject openingPanelGO;
    [SerializeField] private GameObject ingamePanelGO;
    [SerializeField] private GameObject gameOverPanelGO;

    private GameOverPanel gameOverPanel;

    private void Awake()
    {
        EventManager.StartListening(ConstantNames.ShowOpeningPanel, DisplayOpeningPanel);
        EventManager.StartListening(ConstantNames.ShowInGamePanel, DisplayInGamePanel);
        EventManager.StartListening(ConstantNames.ShowGameOverPanel, DisplayGameOverPanel);

        GameManager.Instance.ScoreChanged += ScoreChanged;
        GameManager.Instance.StrikeChanged += StrikesChanged;
        GameManager.Instance.AvailableStationsChanged += AvailableStationsChanged;

        gameOverPanel = gameOverPanelGO.GetComponent<GameOverPanel>();
    }

    private void OnDestroy()
    {
        EventManager.StopListening(ConstantNames.ShowOpeningPanel, DisplayOpeningPanel);
        EventManager.StopListening(ConstantNames.ShowInGamePanel, DisplayInGamePanel);
        EventManager.StopListening(ConstantNames.ShowGameOverPanel, DisplayGameOverPanel);

        GameManager.Instance.ScoreChanged -= ScoreChanged;
        GameManager.Instance.StrikeChanged -= StrikesChanged;
        GameManager.Instance.AvailableStationsChanged -= AvailableStationsChanged;
    }

    private void ScoreChanged(int newScore)
    {
        pointsText.text = newScore.ToString("N0");
    }

    private void StrikesChanged(int newStrikeCount)
    {
        strikesText.text = newStrikeCount.ToString("N0");
    }

    private void AvailableStationsChanged(int newStationsCount)
    {
        availableStationsText.text = newStationsCount.ToString();
    }

    private void DisplayOpeningPanel()
    {
        DisplaySpecificPanel(opening: true);
    }

    private void DisplayInGamePanel()
    {
        DisplaySpecificPanel(inGame: true);
        outOfStrikesText.text = GameManager.Instance.MaxStrikes.ToString();
    }

    private void DisplayGameOverPanel()
    {
        gameOverPanel.Set(GameManager.Instance.CurrentGameOverData);
        DisplaySpecificPanel(gameOver: true);
    }

    private void DisplaySpecificPanel(bool opening = false, bool inGame = false, bool gameOver = false)
    {
        openingPanelGO.SetActive(opening);
        ingamePanelGO.SetActive(inGame);
        gameOverPanelGO.SetActive(gameOver);
    }
}
