using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.BookAppointment;

/// <summary>
/// Books a new appointment with a doctor. The patient is not part of the command:
/// it comes from the authenticated token, so a caller cannot book for somebody else.
/// The appointment is created as Pending.
/// </summary>
public record BookAppointmentCommand(
    int DoctorId,
    DateTime AppointmentDate) : IRequest<Result<AppointmentResponse>>;
