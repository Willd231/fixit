using FixitBackend.Domain.Entities;
using FixitBackend.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FixitBackend.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(FixItDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles
        var roles = new[] { "Administrator", "SupportManager", "Technician", "Requester" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create teams
        if (!context.Teams.Any())
        {
            var teams = new[]
            {
                new Team { Name = "IT Support", Description = "General IT and desktop support", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Team { Name = "Network Operations", Description = "Network and connectivity specialists", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Team { Name = "Software Support", Description = "Application issues and fixes", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Team { Name = "Hardware Support", Description = "Hardware diagnostics and repair", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Team { Name = "Security", Description = "Security incidents and investigation", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Teams.AddRange(teams);
            await context.SaveChangesAsync();
        }

        // Create users
        var teams_ = await context.Teams.ToListAsync();

        var adminExists = await userManager.FindByEmailAsync("admin@fixit.local");
        if (adminExists == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@fixit.local",
                Email = "admin@fixit.local",
                FirstName = "Admin",
                LastName = "User",
                DisplayName = "Admin User",
                IsActive = true,
                IsAvailable = true,
                TeamId = teams_[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }

        var managerExists = await userManager.FindByEmailAsync("manager@fixit.local");
        if (managerExists == null)
        {
            var manager = new ApplicationUser
            {
                UserName = "manager@fixit.local",
                Email = "manager@fixit.local",
                FirstName = "Manager",
                LastName = "User",
                DisplayName = "Manager User",
                IsActive = true,
                IsAvailable = true,
                TeamId = teams_[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(manager, "Manager123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager, "SupportManager");
            }
        }

        var technicianExists = await userManager.FindByEmailAsync("tech1@fixit.local");
        if (technicianExists == null)
        {
            var technician = new ApplicationUser
            {
                UserName = "tech1@fixit.local",
                Email = "tech1@fixit.local",
                FirstName = "Tech",
                LastName = "One",
                DisplayName = "Tech One",
                IsActive = true,
                IsAvailable = true,
                TeamId = teams_[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(technician, "Tech123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(technician, "Technician");
            }
        }

        var requesterExists = await userManager.FindByEmailAsync("user@fixit.local");
        if (requesterExists == null)
        {
            var requester = new ApplicationUser
            {
                UserName = "user@fixit.local",
                Email = "user@fixit.local",
                FirstName = "Test",
                LastName = "User",
                DisplayName = "Test User",
                IsActive = true,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(requester, "User123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(requester, "Requester");
            }
        }

        // Create sample tickets
        if (!context.Tickets.Any())
        {
            var requester = await userManager.FindByEmailAsync("user@fixit.local");
            var technician = await userManager.FindByEmailAsync("tech1@fixit.local");

            var tickets = new[]
            {
                new Ticket
                {
                    TicketNumber = "FIX-2025-000001",
                    Title = "Unable to connect to office Wi-Fi",
                    Description = "Several users in the 3rd floor cannot connect to the corporate Wi-Fi. Error says authentication failed.",
                    RequesterName = "Emma Walker",
                    RequesterEmail = "emma.walker@example.com",
                    RequesterId = requester?.Id ?? "",
                    Status = TicketStatus.Open,
                    Priority = TicketPriority.High,
                    Category = TicketCategory.Network,
                    AssignedTeamId = teams_[1].Id,
                    AssignedUserId = technician?.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Ticket
                {
                    TicketNumber = "FIX-2025-000002",
                    Title = "Microsoft Outlook crashes during startup",
                    Description = "Outlook closes immediately on startup for user Miguel R.",
                    RequesterName = "Carlos Ruiz",
                    RequesterEmail = "c.ruiz@example.com",
                    RequesterId = requester?.Id ?? "",
                    Status = TicketStatus.New,
                    Priority = TicketPriority.Medium,
                    Category = TicketCategory.Software,
                    AssignedTeamId = teams_[2].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Ticket
                {
                    TicketNumber = "FIX-2025-000003",
                    Title = "New employee needs account access",
                    Description = "New hire hired today requires access to email, Slack and internal systems.",
                    RequesterName = "HR System",
                    RequesterEmail = "hr@example.com",
                    RequesterId = requester?.Id ?? "",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.High,
                    Category = TicketCategory.AccountAccess,
                    CreatedAt = DateTime.UtcNow.AddHours(-3),
                    UpdatedAt = DateTime.UtcNow.AddHours(-3)
                },
                new Ticket
                {
                    TicketNumber = "FIX-2025-000004",
                    Title = "Printer on second floor is offline",
                    Description = "The HP printer shows offline and won't accept jobs.",
                    RequesterName = "Office Admin",
                    RequesterEmail = "office@example.com",
                    RequesterId = requester?.Id ?? "",
                    Status = TicketStatus.Resolved,
                    Priority = TicketPriority.Low,
                    Category = TicketCategory.Hardware,
                    ResolutionSummary = "Printer was restarted successfully",
                    ResolvedAt = DateTime.UtcNow.AddDays(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Ticket
                {
                    TicketNumber = "FIX-2025-000005",
                    Title = "Potential phishing email reported",
                    Description = "User reported an email asking for credentials. Needs security review.",
                    RequesterName = "Security Team",
                    RequesterEmail = "security@example.com",
                    RequesterId = requester?.Id ?? "",
                    Status = TicketStatus.InProgress,
                    Priority = TicketPriority.Critical,
                    Category = TicketCategory.Security,
                    AssignedTeamId = teams_[4].Id,
                    CreatedAt = DateTime.UtcNow.AddHours(-6),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1)
                }
            };

            context.Tickets.AddRange(tickets);
            await context.SaveChangesAsync();

            // Add activities
            foreach (var ticket in tickets)
            {
                var activity = new TicketActivity
                {
                    TicketId = ticket.Id,
                    UserId = requester?.Id ?? "",
                    ActivityType = TicketActivityType.TicketCreated,
                    Description = "Ticket created",
                    CreatedAt = ticket.CreatedAt
                };
                context.TicketActivities.Add(activity);
            }

            await context.SaveChangesAsync();
        }
    }
}
