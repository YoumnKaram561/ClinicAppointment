using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Queries.GetMyAppointments;

public record GetMyAppointmentsQuery : IRequest<Result<List<AppointmentResponse>>>;
