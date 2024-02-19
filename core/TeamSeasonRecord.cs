namespace SaturnService;

public class TeamSeasonRecord
{
    public string TeamName { get; set; }
    public int TeamNumber { get; set; }
    public int TotalScore { get; set; }
    public double AverageScore { get; set; }
    public int TotalGames { get; set; }

    public override string ToString()
    {
        return $"{TeamName} (Team #{TeamNumber}) - Total Points: {TotalScore}, Average Points/Game: {AverageScore:0.00}, Games Played: {TotalGames}";
    }
}
