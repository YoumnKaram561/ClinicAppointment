using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Doctors.Queries.GetDoctorById;

/// <summary>
/// Reads a single doctor, including one that is no longer active.
/// </summary>
public record GetDoctorByIdQuery(int Id) : IRequest<Result<DoctorResponse>>;
