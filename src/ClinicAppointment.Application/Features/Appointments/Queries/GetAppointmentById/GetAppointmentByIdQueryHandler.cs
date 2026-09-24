using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetAppointmentByIdQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AppointmentResponse>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment is null)
        {
            return Result<AppointmentResponse>.NotFound($"Appointment {request.Id} was not found.");
        }

        // A patient account may only open its own appointment. A doctor account has no patient
        // record and needs to see the appointment it is being asked to confirm or reject.
        if (_currentUser.TryGetPatientId(out var patientId) && appointment.PatientId != patientId)
        {
            return Result<AppointmentResponse>.Forbidden("This appointment belongs to another patient.");
        }

        return Result<AppointmentResponse>.Success(AppointmentResponse.FromEntity(appointment));
    }
}
