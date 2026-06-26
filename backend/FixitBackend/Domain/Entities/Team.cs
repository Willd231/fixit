namespace FixitBackend.Domain.Entities;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
