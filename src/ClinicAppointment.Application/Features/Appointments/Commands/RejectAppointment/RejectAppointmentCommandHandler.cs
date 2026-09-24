using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Commands.RejectAppointment;

public class RejectAppointmentCommandHandler : IRequestHandler<RejectAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IApplicationDbContext _context;

    public RejectAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentResponse>> Handle(RejectAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment is null)
        {
            return Result<AppointmentResponse>.NotFound($"Appointment {request.Id} was not found.");
        }

        if (!AppointmentTransitions.IsAllowed(appointment.Status, AppointmentStatus.Rejected))
        {
            return Result<AppointmentResponse>.BadRequest(
                AppointmentTransitions.ErrorFor(appointment.Id, appointment.Status, AppointmentStatus.Rejected));
        }

        appointment.Status = AppointmentStatus.Rejected;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AppointmentResponse>.Success(AppointmentResponse.FromEntity(appointment));
    }
}
