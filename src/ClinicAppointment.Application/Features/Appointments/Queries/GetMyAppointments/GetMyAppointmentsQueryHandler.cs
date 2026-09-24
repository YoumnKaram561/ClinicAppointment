using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Queries.GetMyAppointments;

public class GetMyAppointmentsQueryHandler : IRequestHandler<GetMyAppointmentsQuery, Result<List<AppointmentResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyAppointmentsQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<AppointmentResponse>>> Handle(GetMyAppointmentsQuery request, CancellationToken cancellationToken)
    {

        if (!_currentUser.TryGetPatientId(out var patientId))
        {
            return Result<List<AppointmentResponse>>.Forbidden("Only a patient account has its own appointments.");
        }

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);

        return Result<List<AppointmentResponse>>.Success(
            appointments.Select(AppointmentResponse.FromEntity).ToList());
    }
}
