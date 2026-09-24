using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.DTOs;

public record AppointmentResponse(
    int Id,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    DateTime AppointmentDate,
    AppointmentStatus Status,
    DateTime CreatedAt)
{
    /// <summary>
    /// Requires the Patient and Doctor navigations to be loaded by the caller.
    /// </summary>
    public static AppointmentResponse FromEntity(Appointment appointment) => new(
        appointment.Id,
        appointment.PatientId,
        appointment.Patient.Name,
        appointment.DoctorId,
        appointment.Doctor.Name,
        appointment.AppointmentDate,
        appointment.Status,
        appointment.CreatedAt);
}
