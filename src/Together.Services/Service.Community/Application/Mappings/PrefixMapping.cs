using AutoMapper;
using Service.Community.Application.Features.FeaturePrefix.Commands;
using Service.Community.Application.Features.FeaturePrefix.Responses;
using Service.Community.Domain.Aggregates.PrefixAggregate;

namespace Service.Community.Application.Mappings;

public sealed class PrefixMapping : Profile
{
    public PrefixMapping()
    {
        CreateMap<CreatePrefixCommand, Prefix>();
        
        CreateMap<Prefix, CreatePrefixResponse>();
        
        CreateMap<Prefix, PrefixViewModel>();
    }
}