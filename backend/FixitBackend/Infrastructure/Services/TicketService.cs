using AutoMapper;
using FixitBackend.Application.DTOs;
using FixitBackend.Application.Interfaces;
using FixitBackend.Domain.Entities;
using FixitBackend.Domain.Enums;
using FixitBackend.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FixitBackend.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly FixItDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TicketService> _logger;

    public TicketService(FixItDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, ILogger<TicketService> logger)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<PaginatedResponse<TicketListDto>> GetTicketsAsync(TicketFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Tickets
            .Include(t => t.AssignedTeam)
            .Include(t => t.AssignedUser)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(t =>
                t.TicketNumber.ToLower().Contains(search) ||
                t.Title.ToLower().Contains(search) ||
                t.Description.ToLower().Contains(search) ||
                t.RequesterName.ToLower().Contains(search) ||
                t.RequesterEmail.ToLower().Contains(search) ||
                (t.AssignedUser != null && t.AssignedUser.DisplayName.ToLower().Contains(search)));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<TicketStatus>(filter.Status, out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        // Priority filter
        if (!string.IsNullOrWhiteSpace(filter.Priority) && Enum.TryParse<TicketPriority>(filter.Priority, out var priority))
        {
            query = query.Where(t => t.Priority == priority);
        }

        // Category filter
        if (!string.IsNullOrWhiteSpace(filter.Category) && Enum.TryParse<TicketCategory>(filter.Category, out var category))
        {
            query = query.Where(t => t.Category == category);
        }

        // Team filter
        if (filter.TeamId.HasValue)
        {
            query = query.Where(t => t.AssignedTeamId == filter.TeamId);
        }

        // Technician filter
        if (!string.IsNullOrWhiteSpace(filter.TechnicianId))
        {
            query = query.Where(t => t.AssignedUserId == filter.TechnicianId);
        }

        // Requester filter
        if (!string.IsNullOrWhiteSpace(filter.RequesterId))
        {
            query = query.Where(t => t.RequesterId == filter.RequesterId);
        }

        // Date range filters
        if (filter.CreatedFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= filter.CreatedFrom);
        }
        if (filter.CreatedTo.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= filter.CreatedTo.Value.AddDays(1));
        }

        // Sorting
        query = filter.SortDirection.ToLower() == "asc"
            ? query.OrderBy(GetSortProperty(filter.SortBy))
            : query.OrderByDescending(GetSortProperty(filter.SortBy));

        // Pagination
        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TicketListDto>
        {
            Items = _mapper.Map<List<TicketListDto>>(items),
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<TicketDetailsDto?> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .Include(t => t.AssignedTeam)
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments.Where(c => !c.IsDeleted))
            .ThenInclude(c => c.User)
            .Include(t => t.Activities)
            .ThenInclude(a => a.User)
            .Include(t => t.Attachments)
            .ThenInclude(a => a.UploadedByUser)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return ticket != null ? _mapper.Map<TicketDetailsDto>(ticket) : null;
    }

    public async Task<TicketDetailsDto?> GetTicketByNumberAsync(string ticketNumber, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets
            .Include(t => t.AssignedTeam)
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments.Where(c => !c.IsDeleted))
            .ThenInclude(c => c.User)
            .Include(t => t.Activities)
            .ThenInclude(a => a.User)
            .Include(t => t.Attachments)
            .ThenInclude(a => a.UploadedByUser)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

        return ticket != null ? _mapper.Map<TicketDetailsDto>(ticket) : null;
    }

    public async Task<TicketDetailsDto> CreateTicketAsync(CreateTicketRequest request, string requesterId, CancellationToken cancellationToken = default)
    {
        // Verify team exists if assigned
        if (request.AssignedTeamId.HasValue)
        {
            var team = await _context.Teams.FindAsync(new object[] { request.AssignedTeamId.Value }, cancellationToken: cancellationToken);
            if (team == null || !team.IsActive)
                throw new InvalidOperationException("Assigned team not found or inactive");
        }

        // Generate ticket number
        var ticketNumber = await GenerateTicketNumberAsync(cancellationToken);

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            Title = request.Title,
            Description = request.Description,
            RequesterName = request.RequesterName,
            RequesterEmail = request.RequesterEmail,
            RequesterId = requesterId,
            Category = Enum.Parse<TicketCategory>(request.Category),
            Priority = Enum.Parse<TicketPriority>(request.Priority),
            Status = TicketStatus.New,
            AssignedTeamId = request.AssignedTeamId,
            AssignedUserId = request.AssignedUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);

        // Add activity
        var activity = new TicketActivity
        {
            TicketId = ticket.Id,
            UserId = requesterId,
            ActivityType = TicketActivityType.TicketCreated,
            Description = $"Ticket created by {request.RequesterName}",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {TicketNumber} created by {RequesterId}", ticketNumber, requesterId);

        return await GetTicketByIdAsync(ticket.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve created ticket");
    }

    public async Task<TicketDetailsDto> UpdateTicketAsync(int id, UpdateTicketRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null || ticket.IsDeleted)
            throw new InvalidOperationException("Ticket not found");

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Cannot update closed ticket");

        var changes = new List<string>();

        if (ticket.Title != request.Title)
        {
            changes.Add($"Title changed from '{ticket.Title}' to '{request.Title}'");
            ticket.Title = request.Title;
        }

        if (ticket.Description != request.Description)
        {
            changes.Add($"Description updated");
            ticket.Description = request.Description;
        }

        if (ticket.Category.ToString() != request.Category && Enum.TryParse<TicketCategory>(request.Category, out var category))
        {
            changes.Add($"Category changed from {ticket.Category} to {category}");
            ticket.Category = category;

            var activity = new TicketActivity
            {
                TicketId = id,
                UserId = userId,
                ActivityType = TicketActivityType.CategoryChanged,
                Description = changes.Last(),
                OldValue = ticket.Category.ToString(),
                NewValue = category.ToString(),
                CreatedAt = DateTime.UtcNow
            };
            _context.TicketActivities.Add(activity);
        }

        ticket.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} updated by {UserId}", id, userId);

        return await GetTicketByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve updated ticket");
    }

    public async Task<TicketDetailsDto> UpdateStatusAsync(int id, string status, string userId, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<TicketStatus>(status, out var newStatus))
            throw new InvalidOperationException("Invalid status");

        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null || ticket.IsDeleted)
            throw new InvalidOperationException("Ticket not found");

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Cannot change status of closed ticket");

        var oldStatus = ticket.Status;
        ticket.Status = newStatus;
        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = id,
            UserId = userId,
            ActivityType = TicketActivityType.StatusChanged,
            Description = $"Status changed from {oldStatus} to {newStatus}",
            OldValue = oldStatus.ToString(),
            NewValue = newStatus.ToString(),
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} status updated to {Status} by {UserId}", id, status, userId);

        return await GetTicketByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve updated ticket");
    }

    public async Task<TicketDetailsDto> AssignTicketAsync(int id, AssignTicketRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null || ticket.IsDeleted)
            throw new InvalidOperationException("Ticket not found");

        if (request.TeamId.HasValue)
        {
            var team = await _context.Teams.FindAsync(new object[] { request.TeamId.Value }, cancellationToken: cancellationToken);
            if (team == null || !team.IsActive)
                throw new InvalidOperationException("Team not found or inactive");
            ticket.AssignedTeamId = request.TeamId;
        }

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            var technician = await _userManager.FindByIdAsync(request.UserId);
            if (technician == null || !technician.IsActive)
                throw new InvalidOperationException("Technician not found or inactive");
            ticket.AssignedUserId = request.UserId;
        }

        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = id,
            UserId = userId,
            ActivityType = TicketActivityType.TechnicianAssigned,
            Description = $"Ticket assigned to {(request.UserId != null ? "technician" : "team")}",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} assigned by {UserId}", id, userId);

        return await GetTicketByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve updated ticket");
    }

    public async Task<TicketDetailsDto> ResolveTicketAsync(int id, ResolveTicketRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null || ticket.IsDeleted)
            throw new InvalidOperationException("Ticket not found");

        ticket.Status = TicketStatus.Resolved;
        ticket.ResolutionSummary = request.ResolutionSummary;
        ticket.ResolvedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = id,
            UserId = userId,
            ActivityType = TicketActivityType.TicketResolved,
            Description = "Ticket resolved",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} resolved by {UserId}", id, userId);

        return await GetTicketByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve updated ticket");
    }

    public async Task<TicketDetailsDto> ReopenTicketAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null || ticket.IsDeleted)
            throw new InvalidOperationException("Ticket not found");

        ticket.Status = TicketStatus.Open;
        ticket.ResolvedAt = null;
        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = id,
            UserId = userId,
            ActivityType = TicketActivityType.TicketReopened,
            Description = "Ticket reopened",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} reopened by {UserId}", id, userId);

        return await GetTicketByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve updated ticket");
    }

    public async Task<TicketDetailsDto> CloseTicketAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null || ticket.IsDeleted)
            throw new InvalidOperationException("Ticket not found");

        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = id,
            UserId = userId,
            ActivityType = TicketActivityType.TicketClosed,
            Description = "Ticket closed",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} closed by {UserId}", id, userId);

        return await GetTicketByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve updated ticket");
    }

    public async Task<bool> DeleteTicketAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (ticket == null)
            return false;

        ticket.IsDeleted = true;
        ticket.DeletedAt = DateTime.UtcNow;

        var activity = new TicketActivity
        {
            TicketId = id,
            UserId = userId,
            ActivityType = TicketActivityType.TicketDeleted,
            Description = "Ticket deleted",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ticket {Id} deleted by {UserId}", id, userId);

        return true;
    }

    private static Func<Ticket, object> GetSortProperty(string sortBy) =>
        sortBy.ToLower() switch
        {
            "ticketnumber" => t => t.TicketNumber,
            "title" => t => t.Title,
            "priority" => t => t.Priority,
            "status" => t => t.Status,
            "updatedat" => t => t.UpdatedAt,
            _ => t => t.CreatedAt
        };

    private async Task<string> GenerateTicketNumberAsync(CancellationToken cancellationToken = default)
    {
        var currentYear = DateTime.UtcNow.Year;
        var lastTicket = await _context.Tickets
            .Where(t => t.TicketNumber.StartsWith($"FIX-{currentYear}"))
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = (lastTicket == null ? 0 : int.Parse(lastTicket.TicketNumber.Split('-')[2])) + 1;
        return $"FIX-{currentYear}-{nextNumber:D6}";
    }
}
