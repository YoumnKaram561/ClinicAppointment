using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Infrastructure.Authentication;
using ClinicAppointment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicAppointment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // The handlers in the Application layer depend on this abstraction, not on EF Core itself.
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Authentication settings live in configuration only; bad values stop start-up with a
        // clear message instead of producing invalid tokens later.
        var jwtSettings = JwtSettings.FromConfiguration(configuration);
        jwtSettings.Validate();

        services.AddSingleton(jwtSettings);
        services.AddSingleton<ITokenGenerator, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        return services;
    }
}
