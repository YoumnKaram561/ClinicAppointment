using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
