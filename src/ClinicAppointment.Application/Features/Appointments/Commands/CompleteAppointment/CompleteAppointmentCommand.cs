using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.CompleteAppointment;

/// <summary>
/// Moves a Confirmed appointment to Completed.
/// </summary>
public record CompleteAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
