using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.SharedKernel.Mediator;

public static class MediatorExtensions
{
    public static void AddCoreMediator(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        
        services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssembly(assembly);
            c.AddOpenBehavior(typeof(ValidationBehavior<,>));
            c.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });
    }
}