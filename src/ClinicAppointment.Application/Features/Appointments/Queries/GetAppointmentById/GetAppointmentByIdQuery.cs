using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Appointments.Queries.GetAppointmentById;

/// <summary>
/// Reads a single appointment together with its patient and doctor.
/// </summary>
public record GetAppointmentByIdQuery(int Id) : IRequest<Result<AppointmentResponse>>;
