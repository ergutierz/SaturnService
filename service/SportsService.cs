using Microsoft.AspNetCore.Mvc;

namespace SaturnService;

public class SportsService(HttpClient httpClient) : ISportsService
{
    public async Task<string> FetchTeamDataAsync(int teamNumber)
    {
        var requestUri = $"https://sports.snoozle.net/search/nfl/searchHandler?fileType=inline&statType=teamStats&season=2020&teamName={teamNumber}";
        var response = await httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadAsStringAsync();
        return data;
    }
}