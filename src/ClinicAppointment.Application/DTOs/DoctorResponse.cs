using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.DTOs;

public record DoctorResponse(
    int Id,
    string Name,
    string Specialization,
    bool IsActive)
{
    public static DoctorResponse FromEntity(Doctor doctor) => new(
        doctor.Id,
        doctor.Name,
        doctor.Specialization,
        doctor.IsActive);
}
