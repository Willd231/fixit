using FixitBackend.Application.DTOs;

namespace FixitBackend.Application.Interfaces;

public interface ITicketService
{
    Task<PaginatedResponse<TicketListDto>> GetTicketsAsync(TicketFilterRequest filter, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto?> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto?> GetTicketByNumberAsync(string ticketNumber, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> CreateTicketAsync(CreateTicketRequest request, string requesterId, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> UpdateTicketAsync(int id, UpdateTicketRequest request, string userId, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> UpdateStatusAsync(int id, string status, string userId, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> AssignTicketAsync(int id, AssignTicketRequest request, string userId, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> ResolveTicketAsync(int id, ResolveTicketRequest request, string userId, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> ReopenTicketAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<TicketDetailsDto> CloseTicketAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteTicketAsync(int id, string userId, CancellationToken cancellationToken = default);
}

public interface ICommentService
{
    Task<List<TicketCommentDto>> GetCommentsAsync(int ticketId, bool includeInternal, CancellationToken cancellationToken = default);
    Task<TicketCommentDto> AddCommentAsync(int ticketId, CreateCommentRequest request, string userId, CancellationToken cancellationToken = default);
    Task<TicketCommentDto> UpdateCommentAsync(int ticketId, int commentId, string content, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommentAsync(int ticketId, int commentId, string userId, CancellationToken cancellationToken = default);
}

public interface IActivityService
{
    Task<List<TicketActivityDto>> GetActivitiesAsync(int ticketId, CancellationToken cancellationToken = default);
}

public interface IAttachmentService
{
    Task<List<TicketAttachmentDto>> GetAttachmentsAsync(int ticketId, CancellationToken cancellationToken = default);
    Task<TicketAttachmentDto> UploadAttachmentAsync(int ticketId, IFormFile file, string userId, CancellationToken cancellationToken = default);
    Task<FileStreamResult?> DownloadAttachmentAsync(int ticketId, int attachmentId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAttachmentAsync(int ticketId, int attachmentId, string userId, CancellationToken cancellationToken = default);
}

public interface ITeamService
{
    Task<List<TeamDto>> GetTeamsAsync(CancellationToken cancellationToken = default);
    Task<TeamDetailsDto?> GetTeamByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TeamDetailsDto> CreateTeamAsync(CreateTeamRequest request, CancellationToken cancellationToken = default);
    Task<TeamDetailsDto> UpdateTeamAsync(int id, UpdateTeamRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTeamAsync(int id, CancellationToken cancellationToken = default);
    Task<List<UserDto>> GetTeamMembersAsync(int id, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TicketListDto>> GetTeamTicketsAsync(int id, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

public interface IUserService
{
    Task<PaginatedResponse<UserDto>> GetUsersAsync(string? search, string? role, int? teamId, bool? isActive, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<UserDetailsDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserDetailsDto?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(string id, UserDto request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAvailabilityAsync(string id, bool isAvailable, CancellationToken cancellationToken = default);
    Task<bool> AssignToTeamAsync(string id, int teamId, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<TicketListDto>> GetUserTicketsAsync(string id, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<DashboardStatisticsDto> GetStatisticsAsync(string? userId = null, CancellationToken cancellationToken = default);
    Task<List<TicketTrendDto>> GetTicketTrendsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<List<TechnicianWorkloadDto>> GetTechnicianWorkloadAsync(int? teamId = null, CancellationToken cancellationToken = default);
}

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
