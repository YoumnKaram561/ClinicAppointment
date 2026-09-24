using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Doctors.Queries.GetDoctors;

/// <summary>
/// Reads the doctors a patient can book, i.e. the active ones.
/// </summary>
public record GetDoctorsQuery : IRequest<Result<List<DoctorResponse>>>;
