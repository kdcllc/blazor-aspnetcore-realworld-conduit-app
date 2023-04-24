using Bet.BuildingBlocks.Application.Abstractions.Behaviors;
using ConduitApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConduitAppInfraServiceCollectionExtensions
{
    public static IServiceCollection AddDb(this IServiceCollection services)
    {
        // adding pipelines for Mediator
        // extra logging
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // fluent validation
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

        // ef core transaction context
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        services.AddDbContext<ConduitAppContext>(builder =>
        {
            // builder.UseSqlServer(
            //     "Server=localhost,1433;Database=RealworldDb;User=sa;Password=2fb038c8-232c-425c-be0f-f72bfb01bb8a;Encrypt=False",
            //     c =>
            //     {
            //         c.EnableRetryOnFailure();
            //     });
            builder.UseSqlite("Filename=realworld.db");
        });

        return services;
    }
}
