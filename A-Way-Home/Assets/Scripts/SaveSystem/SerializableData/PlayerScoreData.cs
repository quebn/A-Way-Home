using System;
[System.Serializable] 
public class PlayerScoreData : IComparable<PlayerScoreData>
{
    public string charName;
    public string playerName;
    public int score;
    // private uint rank;

    public PlayerScoreData(string charName, string playerName, int score)
    {
        this.charName = charName;
        this.playerName = playerName;
        this.score = score;
        // this.rank = ScoreSystem.GenerateRank(score);
    }

    public int CompareTo(PlayerScoreData other)
    {
        if (other == null ||this.score < other.score)
            return 1;
        else if (this.score > other.score)
            return -1;
        else 
            return 0;
    }
}