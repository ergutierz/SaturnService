using Microsoft.AspNetCore.Mvc;

namespace SaturnService;

[ApiController]
[Route("[controller]")]
public class TeamsController(TeamManager teamManager) : ControllerBase
{
    [HttpGet("GetAllTeamsData")]
    public async Task<IActionResult> GetAllTeamsData()
    {
        try
        {
            var teamsData = await teamManager.ProcessAllTeamsDataAsync();
            return Ok(teamsData);
        }
        catch (Exception ex)
        {
            // Log the exception details
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}