using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.DTOs;

/// <summary>
/// Account details the API is allowed to expose. PasswordHash is deliberately absent.
/// </summary>
public record UserResponse(
    int Id,
    string Name,
    string Email,
    UserRole Role,
    int? PatientId)
{
    public static UserResponse FromEntity(User user) =>
        new(user.Id, user.Name, user.Email, user.Role, user.PatientId);
}
