using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highscoreText;
    [SerializeField] private GameObject newHighScore;
    [SerializeField] private GameObject againButton;

    public void Set(GameOverData gameOverdata)//int score, int highscore, bool isNewHighscore)
    {
        againButton.SetActive(false);
        scoreText.text = gameOverdata.Points.ToString("N0");
        highscoreText.text = gameOverdata.Highscore.ToString("N0");
        newHighScore.SetActive(gameOverdata.IsNewHigh);
    }

    private void OnEnable()
    {
        StartCoroutine(ShowAgainButton());
    }

    private IEnumerator ShowAgainButton()
    {
        yield return new WaitForSeconds(2f);
        againButton.SetActive(true);
    }

    public void New()
    {
        //GameManager.Instance.NewGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
