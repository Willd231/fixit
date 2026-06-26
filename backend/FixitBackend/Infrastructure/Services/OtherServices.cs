using AutoMapper;
using FixitBackend.Application.DTOs;
using FixitBackend.Application.Interfaces;
using FixitBackend.Domain.Entities;
using FixitBackend.Domain.Enums;
using FixitBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixitBackend.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly FixItDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CommentService> _logger;

    public CommentService(FixItDbContext context, IMapper mapper, ILogger<CommentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TicketCommentDto>> GetCommentsAsync(int ticketId, bool includeInternal, CancellationToken cancellationToken = default)
    {
        var query = _context.TicketComments
            .Include(c => c.User)
            .Where(c => c.TicketId == ticketId && !c.IsDeleted);

        if (!includeInternal)
            query = query.Where(c => !c.IsInternal);

        var comments = await query.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);
        return _mapper.Map<List<TicketCommentDto>>(comments);
    }

    public async Task<TicketCommentDto> AddCommentAsync(int ticketId, CreateCommentRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var comment = new TicketComment
        {
            TicketId = ticketId,
            UserId = userId,
            Content = request.Content,
            IsInternal = request.IsInternal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TicketComments.Add(comment);

        var activity = new TicketActivity
        {
            TicketId = ticketId,
            UserId = userId,
            ActivityType = TicketActivityType.CommentAdded,
            Description = "Comment added",
            CreatedAt = DateTime.UtcNow
        };
        _context.TicketActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Comment added to ticket {TicketId} by {UserId}", ticketId, userId);

        return _mapper.Map<TicketCommentDto>(comment);
    }

    public async Task<TicketCommentDto> UpdateCommentAsync(int ticketId, int commentId, string content, string userId, CancellationToken cancellationToken = default)
    {
        var comment = await _context.TicketComments.FirstOrDefaultAsync(c => c.Id == commentId && c.TicketId == ticketId && !c.IsDeleted, cancellationToken);
        if (comment == null)
            throw new InvalidOperationException("Comment not found");

        comment.Content = content;
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TicketCommentDto>(comment);
    }

    public async Task<bool> DeleteCommentAsync(int ticketId, int commentId, string userId, CancellationToken cancellationToken = default)
    {
        var comment = await _context.TicketComments.FirstOrDefaultAsync(c => c.Id == commentId && c.TicketId == ticketId, cancellationToken);
        if (comment == null)
            return false;

        comment.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class ActivityService : IActivityService
{
    private readonly FixItDbContext _context;
    private readonly IMapper _mapper;

    public ActivityService(FixItDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TicketActivityDto>> GetActivitiesAsync(int ticketId, CancellationToken cancellationToken = default)
    {
        var activities = await _context.TicketActivities
            .Include(a => a.User)
            .Where(a => a.TicketId == ticketId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<TicketActivityDto>>(activities);
    }
}

public class TeamService : ITeamService
{
    private readonly FixItDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TeamService> _logger;

    public TeamService(FixItDbContext context, IMapper mapper, ILogger<TeamService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TeamDto>> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        var teams = await _context.Teams.Where(t => t.IsActive).ToListAsync(cancellationToken);
        return _mapper.Map<List<TeamDto>>(teams);
    }

    public async Task<TeamDetailsDto?> GetTeamByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (team == null) return null;

        var dto = _mapper.Map<TeamDetailsDto>(team);
        dto.Members = _mapper.Map<List<UserDto>>(team.Members);
        dto.OpenTicketCount = await _context.Tickets.CountAsync(t => t.AssignedTeamId == id && t.Status != TicketStatus.Closed && t.Status != TicketStatus.Resolved, cancellationToken);
        dto.ResolvedTicketCount = await _context.Tickets.CountAsync(t => t.AssignedTeamId == id && t.Status == TicketStatus.Resolved, cancellationToken);

        return dto;
    }

    public async Task<TeamDetailsDto> CreateTeamAsync(CreateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var team = new Team
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Team {TeamName} created", request.Name);

        return (await GetTeamByIdAsync(team.Id, cancellationToken)) ?? throw new InvalidOperationException("Failed to create team");
    }

    public async Task<TeamDetailsDto> UpdateTeamAsync(int id, UpdateTeamRequest request, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (team == null)
            throw new InvalidOperationException("Team not found");

        team.Name = request.Name;
        team.Description = request.Description;
        team.IsActive = request.IsActive;
        team.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetTeamByIdAsync(id, cancellationToken)) ?? throw new InvalidOperationException("Failed to update team");
    }

    public async Task<bool> DeleteTeamAsync(int id, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (team == null) return false;

        team.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<UserDto>> GetTeamMembersAsync(int id, CancellationToken cancellationToken = default)
    {
        var members = await _context.Users.Where(u => u.TeamId == id && u.IsActive).ToListAsync(cancellationToken);
        return _mapper.Map<List<UserDto>>(members);
    }

    public async Task<PaginatedResponse<TicketListDto>> GetTeamTicketsAsync(int id, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.Tickets
            .Where(t => t.AssignedTeamId == id)
            .OrderByDescending(t => t.CreatedAt);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TicketListDto>
        {
            Items = new List<TicketListDto>(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}

public class UserService : IUserService
{
    private readonly FixItDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(FixItDbContext context, IMapper mapper, ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResponse<UserDto>> GetUsersAsync(int page = 1, int pageSize = 20, string? search = null, string? role = null, int? teamId = null, bool? isActive = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.DisplayName.Contains(search) || u.Email.Contains(search));

        if (teamId.HasValue)
            query = query.Where(u => u.TeamId == teamId);

        var totalItems = await query.CountAsync(cancellationToken);
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<UserDto>
        {
            Items = _mapper.Map<List<UserDto>>(users),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null) return null;

        var dto = _mapper.Map<UserDetailsDto>(user);
        dto.AssignedTicketCount = await _context.Tickets.CountAsync(t => t.AssignedUserId == id, cancellationToken);
        dto.OpenTicketCount = await _context.Tickets.CountAsync(t => t.AssignedUserId == id && t.Status != TicketStatus.Closed && t.Status != TicketStatus.Resolved, cancellationToken);
        dto.ResolvedTicketCount = await _context.Tickets.CountAsync(t => t.AssignedUserId == id && t.Status == TicketStatus.Resolved, cancellationToken);

        return dto;
    }

    public async Task<UserDetailsDto?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await GetUserByIdAsync(userId, cancellationToken);
    }

    public async Task<UserDto?> UpdateUserAsync(string id, UserDto request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DisplayName = request.DisplayName;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> UpdateAvailabilityAsync(string id, bool isAvailable, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (user == null) return false;

        user.IsAvailable = isAvailable;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} availability updated to {IsAvailable}", id, isAvailable);

        return true;
    }

    public async Task<bool> AssignToTeamAsync(string userId, int teamId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken: cancellationToken);
        if (user == null) return false;

        var team = await _context.Teams.FindAsync(new object[] { teamId }, cancellationToken: cancellationToken);
        if (team == null) return false;

        user.TeamId = teamId;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} assigned to team {TeamId}", userId, teamId);

        return true;
    }

    public async Task<PaginatedResponse<TicketListDto>> GetUserTicketsAsync(string userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _context.Tickets
            .Where(t => t.AssignedUserId == userId)
            .OrderByDescending(t => t.CreatedAt);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TicketListDto>
        {
            Items = new List<TicketListDto>(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}

public class DashboardService : IDashboardService
{
    private readonly FixItDbContext _context;
    private readonly IMapper _mapper;

    public DashboardService(FixItDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DashboardStatisticsDto> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        startDate ??= DateTime.UtcNow.AddMonths(-1);
        endDate ??= DateTime.UtcNow;

        var tickets = _context.Tickets.Where(t => !t.IsDeleted).AsQueryable();

        return new DashboardStatisticsDto
        {
            TotalTickets = await tickets.CountAsync(cancellationToken),
            OpenTickets = await tickets.CountAsync(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.New, cancellationToken),
            NewTickets = await tickets.CountAsync(t => t.Status == TicketStatus.New, cancellationToken),
            InProgressTickets = await tickets.CountAsync(t => t.Status == TicketStatus.InProgress, cancellationToken),
            PendingTickets = await tickets.CountAsync(t => t.Status == TicketStatus.Pending, cancellationToken),
            CriticalTickets = await tickets.CountAsync(t => t.Priority == TicketPriority.Critical, cancellationToken),
            UnassignedTickets = await tickets.CountAsync(t => t.AssignedUserId == null, cancellationToken),
            ResolvedTickets = await tickets.CountAsync(t => t.Status == TicketStatus.Resolved, cancellationToken),
            ResolvedThisWeek = await tickets.CountAsync(t => t.ResolvedAt >= DateTime.UtcNow.AddDays(-7), cancellationToken),
            AverageResolutionTime = await GetAverageResolutionTimeAsync(tickets, cancellationToken),
            RecentTickets = new List<TicketListDto>(),
            MyTickets = new List<TicketListDto>(),
            TicketsByStatus = new Dictionary<string, int>
            {
                { "New", await tickets.CountAsync(t => t.Status == TicketStatus.New, cancellationToken) },
                { "Open", await tickets.CountAsync(t => t.Status == TicketStatus.Open, cancellationToken) },
                { "InProgress", await tickets.CountAsync(t => t.Status == TicketStatus.InProgress, cancellationToken) },
                { "Pending", await tickets.CountAsync(t => t.Status == TicketStatus.Pending, cancellationToken) },
                { "Resolved", await tickets.CountAsync(t => t.Status == TicketStatus.Resolved, cancellationToken) },
                { "Closed", await tickets.CountAsync(t => t.Status == TicketStatus.Closed, cancellationToken) }
            },
            TicketsByPriority = new Dictionary<string, int>
            {
                { "Low", await tickets.CountAsync(t => t.Priority == TicketPriority.Low, cancellationToken) },
                { "Medium", await tickets.CountAsync(t => t.Priority == TicketPriority.Medium, cancellationToken) },
                { "High", await tickets.CountAsync(t => t.Priority == TicketPriority.High, cancellationToken) },
                { "Critical", await tickets.CountAsync(t => t.Priority == TicketPriority.Critical, cancellationToken) }
            },
            TicketsByCategory = new Dictionary<string, int>
            {
                { "Hardware", await tickets.CountAsync(t => t.Category == TicketCategory.Hardware, cancellationToken) },
                { "Software", await tickets.CountAsync(t => t.Category == TicketCategory.Software, cancellationToken) },
                { "Network", await tickets.CountAsync(t => t.Category == TicketCategory.Network, cancellationToken) },
                { "AccountAccess", await tickets.CountAsync(t => t.Category == TicketCategory.AccountAccess, cancellationToken) },
                { "Security", await tickets.CountAsync(t => t.Category == TicketCategory.Security, cancellationToken) },
                { "Email", await tickets.CountAsync(t => t.Category == TicketCategory.Email, cancellationToken) },
                { "Other", await tickets.CountAsync(t => t.Category == TicketCategory.Other, cancellationToken) }
            }
        };
    }

    public async Task<List<TicketTrendDto>> GetTicketTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        startDate ??= DateTime.UtcNow.AddDays(-30);
        endDate ??= DateTime.UtcNow;

        var trends = new List<TicketTrendDto>();
        
        for (var date = startDate.Value.Date; date <= endDate.Value.Date; date = date.AddDays(1))
        {
            var created = await _context.Tickets
                .CountAsync(t => t.CreatedAt.Date == date && !t.IsDeleted, cancellationToken);
            
            var resolved = await _context.Tickets
                .CountAsync(t => t.ResolvedAt.HasValue && t.ResolvedAt.Value.Date == date, cancellationToken);

            if (created > 0 || resolved > 0)
            {
                trends.Add(new TicketTrendDto
                {
                    Date = date,
                    Created = created,
                    Resolved = resolved
                });
            }
        }

        return trends;
    }

    public async Task<List<TechnicianWorkloadDto>> GetTechnicianWorkloadAsync(CancellationToken cancellationToken = default)
    {
        var technicians = await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);

        var workload = new List<TechnicianWorkloadDto>();

        foreach (var tech in technicians)
        {
            var assignedTickets = await _context.Tickets
                .CountAsync(t => t.AssignedUserId == tech.Id && !t.IsDeleted, cancellationToken);
            
            var openTickets = await _context.Tickets
                .CountAsync(t => t.AssignedUserId == tech.Id && 
                           (t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress) && 
                           !t.IsDeleted, cancellationToken);
            
            var resolvedTickets = await _context.Tickets
                .CountAsync(t => t.AssignedUserId == tech.Id && t.Status == TicketStatus.Resolved, cancellationToken);

            workload.Add(new TechnicianWorkloadDto
            {
                UserId = tech.Id,
                Name = tech.DisplayName,
                AssignedTickets = assignedTickets,
                OpenTickets = openTickets,
                ResolvedTickets = resolvedTickets,
                AverageResolutionTime = 0 // Could calculate this if needed
            });
        }

        return workload;
    }

    private async Task<double> GetAverageResolutionTimeAsync(IQueryable<Ticket> tickets, CancellationToken cancellationToken = default)
    {
        var resolvedTickets = await tickets
            .Where(t => t.ResolvedAt.HasValue)
            .Select(t => new { t.CreatedAt, t.ResolvedAt })
            .ToListAsync(cancellationToken);

        if (!resolvedTickets.Any()) return 0;

        var totalHours = resolvedTickets
            .Sum(t => (t.ResolvedAt.Value - t.CreatedAt).TotalHours);

        return totalHours / resolvedTickets.Count;
    }
}
