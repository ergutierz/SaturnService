using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace SaturnService;

public class TeamManager
{
    private readonly ISportsService _sportsService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TeamManager> _logger;

    public TeamManager(ISportsService sportsService, IMemoryCache cache, ILogger<TeamManager> logger)
    {
        _sportsService = sportsService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<TeamStat>> ProcessTeamDataAsync(int teamNumber, string correlationId)
    {
        var teamRecords = new List<TeamStat>();
        _logger.LogInformation($"Starting processing for team number {teamNumber} with correlation ID {correlationId}.");
        try
        {
            string json = await _sportsService.FetchTeamDataAsync(teamNumber);
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                JsonElement root = document.RootElement;
                JsonElement matchUpStats = root.GetProperty("matchUpStats");

                foreach (JsonElement match in matchUpStats.EnumerateArray())
                {
                    string gameDate = match.GetProperty("date").GetString();
                    if (DateTime.TryParse(gameDate, out DateTime parsedDate))
                    {
                        var visTeamName = match.GetProperty("visTeamName");
                        var visTeamCode = match.GetProperty("visStats").GetProperty("teamCode");
                        var visScore = match.GetProperty("visStats").GetProperty("score");
                        var homeTeamName = match.GetProperty("homeTeamName");
                        var homeTeamCode = match.GetProperty("homeStats").GetProperty("teamCode");
                        var homeScore = match.GetProperty("homeStats").GetProperty("score");

                        teamRecords.Add(new TeamStat
                        {
                            TeamName = visTeamName.ToString(),
                            TeamNumber = visTeamCode.ToString(),
                            TeamScore = visScore.ToString(),
                            GameDate = parsedDate
                        });

                        teamRecords.Add(new TeamStat
                        {
                            TeamName = homeTeamName.ToString(),
                            TeamNumber = homeTeamCode.ToString(),
                            TeamScore = homeScore.ToString(),
                            GameDate = parsedDate
                        });
                    }
                }
                _logger.LogInformation($"Successfully processed data for team number {teamNumber} with correlation ID {correlationId}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred processing team number {teamNumber}: {ex.Message}");
            _logger.LogError(ex, $"An error occurred while processing team number {teamNumber} with correlation ID {correlationId}.");
        }
        _cache.Set(correlationId, teamRecords, TimeSpan.FromMinutes(5));
        return teamRecords;
    }

    public class TeamStat
    {
        public string TeamName { get; set; }
        public string TeamNumber { get; set; }
        public string TeamScore { get; set; }
        public DateTime GameDate { get; set; }
    }
    
    public class TeamTask
    {
        public int TeamNumber { get; set; }
        public string CorrelationId { get; set; }
    }
}
