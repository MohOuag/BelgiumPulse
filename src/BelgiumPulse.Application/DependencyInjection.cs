using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BelgiumPulse.Application.Common.Behaviors;
using System.Reflection;

namespace BelgiumPulse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // MediatR — détecte automatiquement tous les Handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation — détecte automatiquement tous les Validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Behaviors — s'exécutent dans l'ordre pour chaque requête
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}