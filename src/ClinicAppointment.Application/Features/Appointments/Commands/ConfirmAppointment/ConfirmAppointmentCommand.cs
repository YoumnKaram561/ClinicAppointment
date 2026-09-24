using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.ConfirmAppointment;

public record ConfirmAppointmentCommand(int Id) : IRequest<Result<AppointmentResponse>>;
