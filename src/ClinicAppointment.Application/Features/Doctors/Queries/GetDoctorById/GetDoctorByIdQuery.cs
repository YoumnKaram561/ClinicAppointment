using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Doctors.Queries.GetDoctorById;

public record GetDoctorByIdQuery(int Id) : IRequest<Result<DoctorResponse>>;
