using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Commands.BookAppointment;

public record BookAppointmentCommand(
    int DoctorId,
    DateTime AppointmentDate) : IRequest<Result<AppointmentResponse>>;
