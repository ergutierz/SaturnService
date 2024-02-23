namespace SaturnService
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements the ISportsService interface to fetch sports team data using HTTP.
    /// </summary>
    public class SportsService : ISportsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SportsService> _logger;

        /// <summary>
        /// Initializes a new instance of the SportsService class.
        /// </summary>
        /// <param name="httpClient">The HttpClient used for making HTTP requests.</param>
        /// <param name="logger">The logger for logging operations within the service.</param>
        public SportsService(HttpClient httpClient, ILogger<SportsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously fetches data for a specific team by making an HTTP GET request.
        /// </summary>
        /// <param name="teamNumber">The team number to fetch data for.</param>
        /// <returns>A Task resulting in a string containing the team's data.</returns>
        public async Task<string> FetchTeamDataAsync(int teamNumber)
        {
            var requestUri = $"https://sports.snoozle.net/search/nfl/searchHandler?fileType=inline&statType=teamStats&season=2020&teamName={teamNumber}";
            try
            {
                _logger.LogInformation($"Fetching team data for team number {teamNumber}.");
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode(); 
                var data = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Successfully fetched team data for team number {teamNumber}.");
                return data;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"An error occurred when fetching team data for team number {teamNumber} from {requestUri}.");
                throw; 
            }
        }
    }
}