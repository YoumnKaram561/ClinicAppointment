using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Doctors.Queries.GetDoctors;

public class GetDoctorsQueryHandler : IRequestHandler<GetDoctorsQuery, Result<List<DoctorResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetDoctorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<DoctorResponse>>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);

        return Result<List<DoctorResponse>>.Success(doctors.Select(DoctorResponse.FromEntity).ToList());
    }
}
