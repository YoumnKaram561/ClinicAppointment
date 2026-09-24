using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace ClinicAppointment.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the Commands, Queries and their handlers that live in this assembly.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
