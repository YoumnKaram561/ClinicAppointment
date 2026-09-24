using Microsoft.Extensions.Configuration;

namespace ClinicAppointment.Infrastructure.Authentication;

/// <summary>
/// Read from the "Jwt" section of appsettings.json. No key material lives in C# code.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>HS256 needs a 256 bit key, so the secret must be at least 32 characters.</summary>
    public const int MinimumSecretLength = 32;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public int ExpirationInMinutes { get; init; }

    /// <summary>
    /// Reads the values one by one, so the same small class can be used by the API
    /// (token validation) and by Infrastructure (token creation).
    /// </summary>
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

    /// <summary>
    /// Stops the API with a clear message instead of failing later with a confusing
    /// signing or validation error.
    /// </summary>
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
