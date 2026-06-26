using FixitBackend.Domain.Enums;

namespace FixitBackend.Domain.Entities;

public class TicketActivity
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public TicketActivityType ActivityType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Ticket? Ticket { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
