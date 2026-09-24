using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.CancelAppointment;

/// <summary>
/// Cancels a Pending or Confirmed appointment by changing its status.
/// The appointment is never deleted.
/// </summary>
public record CancelAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
