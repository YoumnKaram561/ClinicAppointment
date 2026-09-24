using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Queries.GetMyAppointments;

/// <summary>
/// Reads the appointments of the signed-in patient. No id is passed: the handler
/// takes it from the authenticated token.
/// </summary>
public record GetMyAppointmentsQuery : IRequest<Result<List<AppointmentResponse>>>;
