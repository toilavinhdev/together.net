using AutoMapper;
using Service.Identity.Application.Features.FeatureRole.Responses;
using Service.Identity.Domain.Aggregates.RoleAggregate;

namespace Service.Identity.Application.Mappings;

public sealed class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, CreateRoleResponse>();
        
        CreateMap<Role, RoleViewModel>();
        
        CreateMap<Role, GetRoleResponse>();
    }
}