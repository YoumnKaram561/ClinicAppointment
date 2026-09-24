using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, Result<DoctorResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DoctorResponse>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (doctor is null)
        {
            return Result<DoctorResponse>.NotFound($"Doctor {request.Id} was not found.");
        }

        return Result<DoctorResponse>.Success(DoctorResponse.FromEntity(doctor));
    }
}
