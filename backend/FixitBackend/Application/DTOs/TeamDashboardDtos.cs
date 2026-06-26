namespace FixitBackend.Application.DTOs;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class TeamDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<UserDto> Members { get; set; } = new();
    public int OpenTicketCount { get; set; }
    public int ResolvedTicketCount { get; set; }
    public double AverageResolutionTime { get; set; }
}

public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class DashboardStatisticsDto
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int NewTickets { get; set; }
    public int InProgressTickets { get; set; }
    public int PendingTickets { get; set; }
    public int CriticalTickets { get; set; }
    public int UnassignedTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int ResolvedThisWeek { get; set; }
    public double AverageResolutionTime { get; set; }
    public List<TicketListDto> RecentTickets { get; set; } = new();
    public List<TicketListDto> MyTickets { get; set; } = new();
    public Dictionary<string, int> TicketsByStatus { get; set; } = new();
    public Dictionary<string, int> TicketsByPriority { get; set; } = new();
    public Dictionary<string, int> TicketsByCategory { get; set; } = new();
}

public class TicketTrendDto
{
    public DateTime Date { get; set; }
    public int Created { get; set; }
    public int Resolved { get; set; }
}

public class TechnicianWorkloadDto
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int AssignedTickets { get; set; }
    public int OpenTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public double AverageResolutionTime { get; set; }
}
