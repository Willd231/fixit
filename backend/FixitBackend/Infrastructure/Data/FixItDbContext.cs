using FixitBackend.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FixitBackend.Infrastructure.Data;

public class FixItDbContext : IdentityDbContext<ApplicationUser>
{
    public FixItDbContext(DbContextOptions<FixItDbContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketActivity> TicketActivities { get; set; }
    public DbSet<TicketAttachment> TicketAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Soft delete filter for Tickets
        modelBuilder.Entity<Ticket>()
            .HasQueryFilter(t => !t.IsDeleted);

        // Soft delete filter for Comments
        modelBuilder.Entity<TicketComment>()
            .HasQueryFilter(c => !c.IsDeleted);

        // Team configuration
        modelBuilder.Entity<Team>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Name)
            .IsUnique();
        modelBuilder.Entity<Team>()
            .Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();
        modelBuilder.Entity<Team>()
            .Property(t => t.Description)
            .HasMaxLength(500);

        // Ticket configuration
        modelBuilder.Entity<Ticket>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.TicketNumber)
            .IsUnique();
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.Status);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.Priority);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.Category);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.AssignedTeamId);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.AssignedUserId);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.RequesterId);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.CreatedAt);
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.UpdatedAt);

        modelBuilder.Entity<Ticket>()
            .Property(t => t.Title)
            .HasMaxLength(150)
            .IsRequired();
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Description)
            .HasMaxLength(5000)
            .IsRequired();
        modelBuilder.Entity<Ticket>()
            .Property(t => t.TicketNumber)
            .HasMaxLength(20)
            .IsRequired();

        // Ticket relationships
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Requester)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedTeam)
            .WithMany(tm => tm.Tickets)
            .HasForeignKey(t => t.AssignedTeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedUser)
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // TicketComment configuration
        modelBuilder.Entity<TicketComment>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<TicketComment>()
            .Property(c => c.Content)
            .HasMaxLength(2000)
            .IsRequired();

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.Ticket)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TicketActivity configuration
        modelBuilder.Entity<TicketActivity>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<TicketActivity>()
            .HasIndex(a => a.CreatedAt);

        modelBuilder.Entity<TicketActivity>()
            .HasOne(a => a.Ticket)
            .WithMany(t => t.Activities)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketActivity>()
            .HasOne(a => a.User)
            .WithMany(u => u.ActivityEntries)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TicketAttachment configuration
        modelBuilder.Entity<TicketAttachment>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<TicketAttachment>()
            .Property(a => a.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();
        modelBuilder.Entity<TicketAttachment>()
            .Property(a => a.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<TicketAttachment>()
            .HasOne(a => a.Ticket)
            .WithMany(t => t.Attachments)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketAttachment>()
            .HasOne(a => a.UploadedByUser)
            .WithMany(u => u.Attachments)
            .HasForeignKey(a => a.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.FirstName)
            .HasMaxLength(100);
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.LastName)
            .HasMaxLength(100);
        modelBuilder.Entity<ApplicationUser>()
            .Property(u => u.DisplayName)
            .HasMaxLength(150);

        // Enum serialization
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Priority)
            .HasConversion<string>();
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Category)
            .HasConversion<string>();
        modelBuilder.Entity<TicketActivity>()
            .Property(a => a.ActivityType)
            .HasConversion<string>();
    }
}
