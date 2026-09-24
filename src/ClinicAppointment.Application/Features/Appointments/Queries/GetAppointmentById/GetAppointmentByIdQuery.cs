using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(int Id) : IRequest<Result<AppointmentResponse>>;
