namespace SaturnService
{
    /// <summary>
    /// Represents a request to process data for a specific team.
    /// </summary>
    public class TeamRequest
    {
        /// <summary>
        /// Gets or sets the number of the team for which data is requested.
        /// </summary>
        /// <value>The team's numerical identifier.</value>
        public int TeamNumber { get; set; }
    }
}