using System.Security.Claims;
using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.API.Common;

/// <summary>
/// Reads the signed-in user from the validated JWT claims. The values are produced by
/// the token service at login, so a client cannot change them without breaking the signature.
/// </summary>
public class JwtCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private Claim? GetClaim(string type) => _httpContextAccessor.HttpContext?.User.FindFirst(type);

    public int? UserId =>
        int.TryParse(GetClaim(AuthTokenClaims.Subject)?.Value, out var userId) ? userId : null;

    public UserRole? Role =>
        Enum.TryParse<UserRole>(GetClaim(AuthTokenClaims.Role)?.Value, ignoreCase: true, out var role)
            ? role
            : null;

    /// <summary>Missing for a Doctor token, which is what keeps a doctor out of patient operations.</summary>
    public int? PatientId =>
        int.TryParse(GetClaim(AuthTokenClaims.PatientId)?.Value, out var patientId) ? patientId : null;
}
