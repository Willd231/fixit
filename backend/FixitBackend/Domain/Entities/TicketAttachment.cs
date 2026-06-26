namespace FixitBackend.Domain.Entities;

public class TicketAttachment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string UploadedByUserId { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Ticket? Ticket { get; set; }
    public virtual ApplicationUser? UploadedByUser { get; set; }
}
