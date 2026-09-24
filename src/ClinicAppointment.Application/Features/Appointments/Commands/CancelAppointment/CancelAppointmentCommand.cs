using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
