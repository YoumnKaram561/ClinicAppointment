using Microsoft.Extensions.Configuration;

namespace ClinicAppointment.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public const int MinimumSecretLength = 32;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public int ExpirationInMinutes { get; init; }

    public static JwtSettings FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);

        return new JwtSettings
        {
            Issuer = section[nameof(Issuer)] ?? string.Empty,
            Audience = section[nameof(Audience)] ?? string.Empty,
            SecretKey = section[nameof(SecretKey)] ?? string.Empty,
            ExpirationInMinutes = int.TryParse(section[nameof(ExpirationInMinutes)], out var minutes) ? minutes : 0
        };
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer) || string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException($"{SectionName}:Issuer and {SectionName}:Audience must be configured.");
        }

        if (string.IsNullOrWhiteSpace(SecretKey) || SecretKey.Length < MinimumSecretLength)
        {
            throw new InvalidOperationException(
                $"{SectionName}:SecretKey must be configured and at least {MinimumSecretLength} characters long.");
        }

        if (ExpirationInMinutes <= 0)
        {
            throw new InvalidOperationException($"{SectionName}:ExpirationInMinutes must be greater than zero.");
        }
    }
}
