using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.RejectAppointment;

public record RejectAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
