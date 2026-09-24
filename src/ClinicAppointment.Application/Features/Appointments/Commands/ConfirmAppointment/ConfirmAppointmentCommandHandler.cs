using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Commands.ConfirmAppointment;

public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IApplicationDbContext _context;

    public ConfirmAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentResponse>> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment is null)
        {
            return Result<AppointmentResponse>.NotFound($"Appointment {request.Id} was not found.");
        }

        if (!AppointmentTransitions.IsAllowed(appointment.Status, AppointmentStatus.Confirmed))
        {
            return Result<AppointmentResponse>.BadRequest(
                AppointmentTransitions.ErrorFor(appointment.Id, appointment.Status, AppointmentStatus.Confirmed));
        }

        appointment.Status = AppointmentStatus.Confirmed;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AppointmentResponse>.Success(AppointmentResponse.FromEntity(appointment));
    }
}
