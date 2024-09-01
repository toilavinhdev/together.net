using AutoMapper;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.ValueObjects;
using Service.Identity.Application.Features.FeatureUser.Responses;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.Mappings;

public sealed class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, IdentityPrivilege>()
            .ForMember(dest => dest.Permissions, cfg =>
                cfg.MapFrom(src => src.UserRoles!.SelectMany(ur => ur.Role.Claims).Distinct().ToList()));
        
        CreateMap<User, UserClaimsPrincipal>()
            .ForMember(dest => dest.Permissions, cfg =>
                cfg.MapFrom(src => src.UserRoles!.SelectMany(ur => ur.Role.Claims).Distinct().ToList()));
        
        CreateMap<User, SignUpResponse>();
        
        CreateMap<User, UserViewModel>();
        
        CreateMap<IdentityPrivilege, UserClaimsPrincipal>();

        CreateMap<IdentityPrivilege, MeResponse>();
    }
}