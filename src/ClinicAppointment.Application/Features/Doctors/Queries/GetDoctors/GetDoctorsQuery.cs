using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Doctors.Queries.GetDoctors;

public record GetDoctorsQuery : IRequest<Result<List<DoctorResponse>>>;
