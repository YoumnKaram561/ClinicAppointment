using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.Interfaces;

public interface ICurrentUser
{

    int? UserId { get; }

    UserRole? Role { get; }

    int? PatientId { get; }
}
