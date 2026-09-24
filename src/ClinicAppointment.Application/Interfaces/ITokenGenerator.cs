using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.Interfaces;

/// <summary>
/// Creates the access token for a signed-in account. Implemented in Infrastructure,
/// so the Application layer stays free of JWT and signing details.
/// </summary>
public interface ITokenGenerator
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
