using AutoMapper;
using FixitBackend.Application.DTOs;
using FixitBackend.Domain.Entities;

namespace FixitBackend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Ticket mappings
        CreateMap<Ticket, TicketListDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
            .ForMember(dest => dest.AssignedTeamName, opt => opt.MapFrom(src => src.AssignedTeam!.Name))
            .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.AssignedUser!.DisplayName));

        CreateMap<Ticket, TicketDetailsDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
            .ForMember(dest => dest.AssignedTeamName, opt => opt.MapFrom(src => src.AssignedTeam!.Name))
            .ForMember(dest => dest.AssignedUserName, opt => opt.MapFrom(src => src.AssignedUser!.DisplayName));

        // Comment mappings
        CreateMap<TicketComment, TicketCommentDto>()
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.User!.Id))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User!.DisplayName));

        // Activity mappings
        CreateMap<TicketActivity, TicketActivityDto>()
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType.ToString()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.DisplayName));

        // Attachment mappings
        CreateMap<TicketAttachment, TicketAttachmentDto>()
            .ForMember(dest => dest.UploadedByUserName, opt => opt.MapFrom(src => src.UploadedByUser!.DisplayName));

        // Team mappings
        CreateMap<Team, TeamDto>();
        CreateMap<Team, TeamDetailsDto>();
        CreateMap<CreateTeamRequest, Team>();
        CreateMap<UpdateTeamRequest, Team>();

        // User mappings
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team!.Name));

        CreateMap<ApplicationUser, UserDetailsDto>()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team!.Name));
    }
}
