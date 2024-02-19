namespace SaturnService;

public interface ISportsService
{
    Task<string> FetchTeamDataAsync(int teamNumber);
}