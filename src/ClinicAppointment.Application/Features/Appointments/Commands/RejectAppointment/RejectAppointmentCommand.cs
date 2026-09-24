using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.RejectAppointment;

/// <summary>
/// Moves a Pending appointment to Rejected. The record is kept, never deleted.
/// </summary>
public record RejectAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
