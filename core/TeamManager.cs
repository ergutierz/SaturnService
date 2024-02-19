using System.Text.Json;

namespace SaturnService;

public class TeamManager(ISportsService sportsService)
{
    public async Task<List<TeamStat>> ProcessAllTeamsDataAsync()
    {
        var allTeamRecords = new List<TeamStat>();
        var tasks = new List<Task>();

        for (int teamNumber = 1; teamNumber <= 32; teamNumber++)
        {
            int capturedTeamNumber = teamNumber;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    string json = await sportsService.FetchTeamDataAsync(capturedTeamNumber);
                    using (JsonDocument document = JsonDocument.Parse(json))
                    {
                        JsonElement root = document.RootElement;
                        JsonElement matchUpStats = root.GetProperty("matchUpStats");

                        foreach (JsonElement match in matchUpStats.EnumerateArray())
                        {
                            string gameDate = match.GetProperty("date").GetString();
                            if (DateTime.TryParse(gameDate, out DateTime parsedDate) && parsedDate.Year == 2020)
                            {
                                string visTeamName = match.GetProperty("visTeamName").GetString();
                                string visTeamCode = match.GetProperty("visStats").GetProperty("teamCode").ToString();
                                int visScore = match.GetProperty("visStats").GetProperty("score").GetInt32();
                                string homeTeamName = match.GetProperty("homeTeamName").GetString();
                                string homeTeamCode = match.GetProperty("homeStats").GetProperty("teamCode").ToString();
                                int homeScore = match.GetProperty("homeStats").GetProperty("score").GetInt32();
                                
                                lock (allTeamRecords)
                                {
                                    allTeamRecords.Add(new TeamStat
                                    {
                                        TeamName = visTeamName,
                                        TeamNumber = visTeamCode,
                                        TeamScore = visScore.ToString(),
                                        GameDate = parsedDate
                                    });

                                    allTeamRecords.Add(new TeamStat
                                    {
                                        TeamName = homeTeamName,
                                        TeamNumber = homeTeamCode,
                                        TeamScore = homeScore.ToString(),
                                        GameDate = parsedDate
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred processing team number {capturedTeamNumber}: {ex.Message}");
                }
            }));
        }

        await Task.WhenAll(tasks);

        var distinctTeamRecords = allTeamRecords
            .GroupBy(r => new { r.TeamNumber, r.GameDate })
            .Select(g => g.First())
            .ToList();

        return distinctTeamRecords;
    }
    
    public class TeamStat
    {
        public string TeamName { get; set; }
        public string TeamNumber { get; set; }
        public string TeamScore { get; set; }
        public DateTime GameDate { get; set; }
    }

}