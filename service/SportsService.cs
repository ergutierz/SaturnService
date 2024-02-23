namespace SaturnService;

public class SportsService(HttpClient httpClient, ILogger<SportsService> logger) : ISportsService
{
    public async Task<string> FetchTeamDataAsync(int teamNumber)
    {
        var requestUri = $"https://sports.snoozle.net/search/nfl/searchHandler?fileType=inline&statType=teamStats&season=2020&teamName={teamNumber}";
        try
        {
            logger.LogInformation($"Fetching team data for team number {teamNumber}.");
            var response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode(); // This throws an exception if the HTTP response status is an error code.
            var data = await response.Content.ReadAsStringAsync();
            logger.LogInformation($"Successfully fetched team data for team number {teamNumber}.");
            return data;
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, $"An error occurred when fetching team data for team number {teamNumber} from {requestUri}.");
            throw; 
        }
    }
}