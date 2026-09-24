using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IApplicationDbContext _context;

    public CompleteAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentResponse>> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment is null)
        {
            return Result<AppointmentResponse>.NotFound($"Appointment {request.Id} was not found.");
        }

        if (!AppointmentTransitions.IsAllowed(appointment.Status, AppointmentStatus.Completed))
        {
            return Result<AppointmentResponse>.BadRequest(
                AppointmentTransitions.ErrorFor(appointment.Id, appointment.Status, AppointmentStatus.Completed));
        }

        appointment.Status = AppointmentStatus.Completed;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AppointmentResponse>.Success(AppointmentResponse.FromEntity(appointment));
    }
}
