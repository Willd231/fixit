using FixitBackend.Application.DTOs;
using FixitBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixitBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
    {
        _teamService = teamService;
        _logger = logger;
    }

    /// <summary>
    /// Get all active teams
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TeamDto>>> GetTeams(CancellationToken cancellationToken)
    {
        try
        {
            var teams = await _teamService.GetTeamsAsync(cancellationToken);
            return Ok(teams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teams");
            return BadRequest("Error fetching teams");
        }
    }

    /// <summary>
    /// Get team by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDetailsDto>> GetTeam(int id, CancellationToken cancellationToken)
    {
        try
        {
            var team = await _teamService.GetTeamByIdAsync(id, cancellationToken);
            if (team == null)
                return NotFound("Team not found");

            return Ok(team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team {TeamId}", id);
            return BadRequest("Error fetching team");
        }
    }

    /// <summary>
    /// Create a new team (admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrator,SupportManager")]
    public async Task<ActionResult<TeamDetailsDto>> CreateTeam(CreateTeamRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var team = await _teamService.CreateTeamAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating team");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team");
            return BadRequest("Error creating team");
        }
    }

    /// <summary>
    /// Update a team (admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,SupportManager")]
    public async Task<ActionResult<TeamDetailsDto>> UpdateTeam(int id, UpdateTeamRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var team = await _teamService.UpdateTeamAsync(id, request, cancellationToken);
            return Ok(team);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating team {TeamId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team {TeamId}", id);
            return BadRequest("Error updating team");
        }
    }

    /// <summary>
    /// Delete a team (admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteTeam(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _teamService.DeleteTeamAsync(id, cancellationToken);
            if (!result)
                return NotFound("Team not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team {TeamId}", id);
            return BadRequest("Error deleting team");
        }
    }

    /// <summary>
    /// Get team members
    /// </summary>
    [HttpGet("{id}/members")]
    public async Task<ActionResult<List<UserDto>>> GetTeamMembers(int id, CancellationToken cancellationToken)
    {
        try
        {
            var members = await _teamService.GetTeamMembersAsync(id, cancellationToken);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team members for team {TeamId}", id);
            return BadRequest("Error fetching team members");
        }
    }

    /// <summary>
    /// Get team tickets
    /// </summary>
    [HttpGet("{id}/tickets")]
    public async Task<ActionResult<PaginatedResponse<TicketListDto>>> GetTeamTickets(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var tickets = await _teamService.GetTeamTicketsAsync(id, page, pageSize, cancellationToken);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team tickets for team {TeamId}", id);
            return BadRequest("Error fetching team tickets");
        }
    }
}
