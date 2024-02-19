namespace SaturnService;

public interface SportsService
{
    Task<string> FetchTeamDataAsync(int teamNumber);
}