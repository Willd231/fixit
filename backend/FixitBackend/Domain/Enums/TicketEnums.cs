namespace FixitBackend.Domain.Enums;

public enum TicketStatus
{
    New,
    Open,
    InProgress,
    Pending,
    Resolved,
    Closed
}

public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum TicketCategory
{
    Hardware,
    Software,
    Network,
    AccountAccess,
    Security,
    Email,
    Other
}

public enum TicketActivityType
{
    TicketCreated,
    TicketUpdated,
    StatusChanged,
    PriorityChanged,
    CategoryChanged,
    TeamAssigned,
    TechnicianAssigned,
    CommentAdded,
    AttachmentAdded,
    TicketResolved,
    TicketReopened,
    TicketClosed,
    TicketDeleted
}
