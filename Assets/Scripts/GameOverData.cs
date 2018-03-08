public struct GameOverData
{
    public int Points;
    public int Highscore;
    public bool IsNewHigh;

    public GameOverData(int points, int highScore, bool isNewHigh)
    {
        this.Points = points;
        this.Highscore = highScore;
        this.IsNewHigh = isNewHigh;
    }
}
