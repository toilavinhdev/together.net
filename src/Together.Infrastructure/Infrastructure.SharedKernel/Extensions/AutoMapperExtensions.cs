using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel.Extensions;

public static class AutoMapperExtensions
{
    private static IMapper _mapper = default!;
    
    public static void AddCoreMapper(this IServiceCollection services, Assembly assembly)
    {
        var mapperConfig = new MapperConfiguration(
            cfg =>
            {
                cfg.AddMaps(assembly);
            });

        services.AddSingleton(CreateInstance(mapperConfig));
    }

    private static IMapper CreateInstance(MapperConfiguration configuration)
    {
        _mapper = configuration.CreateMapper();
        return _mapper;
    }
    
    public static TDestination MapTo<TDestination>(this object source)
    {
        return _mapper.Map<TDestination>(source);
    }
    
    public static TDestination MapTo<TDestination>(this object source, 
        Action<IMappingOperationOptions<object, TDestination>> options)
    {
        return _mapper.Map(source, options);
    }
    
    public static TDestination MapTo<TSource, TDestination>(this TSource source, 
        Action<IMappingOperationOptions<TSource, TDestination>> options)
    {
        return _mapper.Map(source, options);
    }
}