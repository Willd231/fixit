using FixitBackend.Application.DTOs;
using FixitBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixitBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ICommentService _commentService;
    private readonly IActivityService _activityService;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(ITicketService ticketService, ICommentService commentService, IActivityService activityService, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _commentService = commentService;
        _activityService = activityService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of tickets with filtering and search
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TicketListDto>>> GetTickets([FromQuery] TicketFilterRequest filter, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ticketService.GetTicketsAsync(filter, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tickets");
            return BadRequest("Error fetching tickets");
        }
    }

    /// <summary>
    /// Get ticket by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDetailsDto>> GetTicket(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id, cancellationToken);
            if (ticket == null)
                return NotFound("Ticket not found");

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ticket {TicketId}", id);
            return BadRequest("Error fetching ticket");
        }
    }

    /// <summary>
    /// Get ticket by ticket number
    /// </summary>
    [HttpGet("number/{ticketNumber}")]
    public async Task<ActionResult<TicketDetailsDto>> GetTicketByNumber(string ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber, cancellationToken);
            if (ticket == null)
                return NotFound("Ticket not found");

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ticket {TicketNumber}", ticketNumber);
            return BadRequest("Error fetching ticket");
        }
    }

    /// <summary>
    /// Create a new ticket
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<TicketDetailsDto>> CreateTicket(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? "anonymous";
            var ticket = await _ticketService.CreateTicketAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating ticket");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ticket");
            return BadRequest("Error creating ticket");
        }
    }

    /// <summary>
    /// Update a ticket
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator,SupportManager,Technician")]
    public async Task<ActionResult<TicketDetailsDto>> UpdateTicket(int id, UpdateTicketRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var ticket = await _ticketService.UpdateTicketAsync(id, request, userId, cancellationToken);
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating ticket {TicketId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket {TicketId}", id);
            return BadRequest("Error updating ticket");
        }
    }

    /// <summary>
    /// Update ticket status
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Administrator,SupportManager,Technician")]
    public async Task<ActionResult<TicketDetailsDto>> UpdateStatus(int id, UpdateTicketStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var ticket = await _ticketService.UpdateStatusAsync(id, request.Status, userId, cancellationToken);
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error updating status for ticket {TicketId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket status {TicketId}", id);
            return BadRequest("Error updating status");
        }
    }

    /// <summary>
    /// Assign ticket to team/technician
    /// </summary>
    [HttpPatch("{id}/assignment")]
    [Authorize(Roles = "Administrator,SupportManager")]
    public async Task<ActionResult<TicketDetailsDto>> AssignTicket(int id, AssignTicketRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var ticket = await _ticketService.AssignTicketAsync(id, request, userId, cancellationToken);
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error assigning ticket {TicketId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning ticket {TicketId}", id);
            return BadRequest("Error assigning ticket");
        }
    }

    /// <summary>
    /// Resolve a ticket
    /// </summary>
    [HttpPost("{id}/resolve")]
    [Authorize(Roles = "Administrator,SupportManager,Technician")]
    public async Task<ActionResult<TicketDetailsDto>> ResolveTicket(int id, ResolveTicketRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var ticket = await _ticketService.ResolveTicketAsync(id, request, userId, cancellationToken);
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error resolving ticket {TicketId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving ticket {TicketId}", id);
            return BadRequest("Error resolving ticket");
        }
    }

    /// <summary>
    /// Reopen a resolved ticket
    /// </summary>
    [HttpPost("{id}/reopen")]
    [Authorize(Roles = "Administrator,SupportManager,Technician")]
    public async Task<ActionResult<TicketDetailsDto>> ReopenTicket(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var ticket = await _ticketService.ReopenTicketAsync(id, userId, cancellationToken);
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error reopening ticket {TicketId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reopening ticket {TicketId}", id);
            return BadRequest("Error reopening ticket");
        }
    }

    /// <summary>
    /// Close a ticket
    /// </summary>
    [HttpPost("{id}/close")]
    [Authorize(Roles = "Administrator,SupportManager")]
    public async Task<ActionResult<TicketDetailsDto>> CloseTicket(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var ticket = await _ticketService.CloseTicketAsync(id, userId, cancellationToken);
            return Ok(ticket);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error closing ticket {TicketId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing ticket {TicketId}", id);
            return BadRequest("Error closing ticket");
        }
    }

    /// <summary>
    /// Delete (soft delete) a ticket
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteTicket(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var result = await _ticketService.DeleteTicketAsync(id, userId, cancellationToken);
            if (!result)
                return NotFound("Ticket not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ticket {TicketId}", id);
            return BadRequest("Error deleting ticket");
        }
    }

    /// <summary>
    /// Get comments for a ticket
    /// </summary>
    [HttpGet("{ticketId}/comments")]
    public async Task<ActionResult<List<TicketCommentDto>>> GetComments(int ticketId, CancellationToken cancellationToken)
    {
        try
        {
            var isStaff = User.IsInRole("Administrator") || User.IsInRole("SupportManager") || User.IsInRole("Technician");
            var comments = await _commentService.GetCommentsAsync(ticketId, isStaff, cancellationToken);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching comments for ticket {TicketId}", ticketId);
            return BadRequest("Error fetching comments");
        }
    }

    /// <summary>
    /// Add a comment to a ticket
    /// </summary>
    [HttpPost("{ticketId}/comments")]
    [Authorize]
    public async Task<ActionResult<TicketCommentDto>> AddComment(int ticketId, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            var comment = await _commentService.AddCommentAsync(ticketId, request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetComments), new { ticketId }, comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to ticket {TicketId}", ticketId);
            return BadRequest("Error adding comment");
        }
    }

    /// <summary>
    /// Get activity history for a ticket
    /// </summary>
    [HttpGet("{ticketId}/activities")]
    public async Task<ActionResult<List<TicketActivityDto>>> GetActivities(int ticketId, CancellationToken cancellationToken)
    {
        try
        {
            var activities = await _activityService.GetActivitiesAsync(ticketId, cancellationToken);
            return Ok(activities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching activities for ticket {TicketId}", ticketId);
            return BadRequest("Error fetching activities");
        }
    }
}
