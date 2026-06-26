using FixitBackend.Application.DTOs;
using FluentValidation;

namespace FixitBackend.Application.Validators;

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(5, 150).WithMessage("Title must be between 5 and 150 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(10, 5000).WithMessage("Description must be between 10 and 5000 characters");

        RuleFor(x => x.RequesterName)
            .NotEmpty().WithMessage("Requester name is required")
            .Length(2, 100).WithMessage("Requester name must be between 2 and 100 characters");

        RuleFor(x => x.RequesterEmail)
            .NotEmpty().WithMessage("Requester email is required")
            .EmailAddress().WithMessage("Valid email is required");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(IsValidCategory).WithMessage("Invalid category");

        RuleFor(x => x.Priority)
            .Must(IsValidPriority).WithMessage("Invalid priority");
    }

    private static bool IsValidCategory(string category)
    {
        var validCategories = new[] { "Hardware", "Software", "Network", "AccountAccess", "Security", "Email", "Other" };
        return validCategories.Contains(category);
    }

    private static bool IsValidPriority(string priority)
    {
        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        return validPriorities.Contains(priority);
    }
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .Length(1, 2000).WithMessage("Comment must not exceed 2000 characters");
    }
}

public class ResolveTicketRequestValidator : AbstractValidator<ResolveTicketRequest>
{
    public ResolveTicketRequestValidator()
    {
        RuleFor(x => x.ResolutionSummary)
            .NotEmpty().WithMessage("Resolution summary is required")
            .Length(10, 2000).WithMessage("Resolution summary must be between 10 and 2000 characters");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Valid email is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
