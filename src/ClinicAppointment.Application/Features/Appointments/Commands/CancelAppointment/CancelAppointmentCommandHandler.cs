using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CancelAppointmentCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AppointmentResponse>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment is null)
        {
            return Result<AppointmentResponse>.NotFound($"Appointment {request.Id} was not found.");
        }

        if (!_currentUser.TryGetPatientId(out var patientId))
        {
            return Result<AppointmentResponse>.Forbidden("Only a patient account can cancel an appointment.");
        }

        if (appointment.PatientId != patientId)
        {
            return Result<AppointmentResponse>.Forbidden("This appointment belongs to another patient.");
        }

        if (!AppointmentTransitions.IsAllowed(appointment.Status, AppointmentStatus.Cancelled))
        {
            return Result<AppointmentResponse>.BadRequest(
                AppointmentTransitions.ErrorFor(appointment.Id, appointment.Status, AppointmentStatus.Cancelled));
        }

        appointment.Status = AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AppointmentResponse>.Success(AppointmentResponse.FromEntity(appointment));
    }
}
