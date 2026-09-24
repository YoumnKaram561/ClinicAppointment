using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace ClinicAppointment.Infrastructure.Authentication;

/// <summary>
/// Builds a signed JWT (HS256) for an account. The claims carry the ids the API needs
/// later, so no request has to send them again.
/// </summary>
public class JwtTokenService : ITokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(_settings.ExpirationInMinutes);

        var claims = new List<Claim>
        {
            new(AuthTokenClaims.Subject, user.Id.ToString()),
            new(AuthTokenClaims.Name, user.Name),
            new(AuthTokenClaims.Email, user.Email),
            new(AuthTokenClaims.Role, user.Role.ToString()),
        };

        // Only patient accounts get this claim; a doctor token cannot act as a patient.
        if (user.PatientId is not null)
        {
            claims.Add(new Claim(AuthTokenClaims.PatientId, user.PatientId.Value.ToString()));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
