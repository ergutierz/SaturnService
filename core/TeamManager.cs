using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace SaturnService
{
    /// <summary>
    /// Manages the processing of team data, including fetching, parsing, and caching.
    /// </summary>
    public class TeamManager
    {
        private readonly ISportsService _sportsService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TeamManager> _logger;

        /// <summary>
        /// Initializes a new instance of the TeamManager class.
        /// </summary>
        /// <param name="sportsService">The sports service for fetching team data.</param>
        /// <param name="cache">The cache for storing processed team data.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        public TeamManager(ISportsService sportsService, IMemoryCache cache, ILogger<TeamManager> logger)
        {
            _sportsService = sportsService;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Processes data for a specific team based on the team number and caches the results.
        /// </summary>
        /// <param name="teamNumber">The team number to process data for.</param>
        /// <param name="correlationId">A unique identifier for the processing request.</param>
        /// <returns>A list of processed team statistics.</returns>
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
                }
                _logger.LogInformation($"Successfully processed data for team number {teamNumber} with correlation ID {correlationId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing team number {teamNumber} with correlation ID {correlationId}.");
            }
            _cache.Set(correlationId, teamRecords, TimeSpan.FromMinutes(5)); // Cache the processed data
            return teamRecords;
        }

        /// <summary>
        /// Represents statistics for a team.
        /// </summary>
        public class TeamStat
        {
            public string TeamName { get; set; }
            public string TeamNumber { get; set; }
            public string TeamScore { get; set; }
            public DateTime GameDate { get; set; }
        }

        /// <summary>
        /// Represents a task associated with processing team data.
        /// </summary>
        public class TeamTask
        {
            public int TeamNumber { get; set; }
            public string CorrelationId { get; set; }
        }
    }
}

