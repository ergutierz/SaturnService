namespace SaturnService
{
    /// <summary>
    /// Defines the interface for a service that fetches sports team data.
    /// </summary>
    public interface ISportsService
    {
        /// <summary>
        /// Asynchronously fetches data for a specific team.
        /// </summary>
        /// <param name="teamNumber">The team number to fetch data for.</param>
        /// <returns>A Task resulting in a string containing the team's data.</returns>
        Task<string> FetchTeamDataAsync(int teamNumber);
    }
}