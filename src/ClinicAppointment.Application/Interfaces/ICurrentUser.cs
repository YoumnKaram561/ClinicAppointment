using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.Interfaces;

/// <summary>
/// Who is calling, taken from the authenticated JWT - never from a value in the request.
/// Implemented in the API layer on top of HttpContext.User.
/// </summary>
public interface ICurrentUser
{
    /// <summary>Id of the signed-in account, null when the request is anonymous.</summary>
    int? UserId { get; }

    UserRole? Role { get; }

    /// <summary>
    /// Patient record of the signed-in account. Null for a Doctor account,
    /// which is why patient operations cannot run with a doctor token.
    /// </summary>
    int? PatientId { get; }
}
