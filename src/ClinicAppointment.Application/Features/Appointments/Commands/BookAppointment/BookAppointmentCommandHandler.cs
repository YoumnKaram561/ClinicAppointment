using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Result<AppointmentResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public BookAppointmentCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AppointmentResponse>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {

        if (!_currentUser.TryGetPatientId(out var patientId))
        {
            return Result<AppointmentResponse>.Forbidden("Only a patient account can book an appointment.");
        }

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);

        if (patient is null)
        {
            return Result<AppointmentResponse>.NotFound("The patient record of this account was not found.");
        }

        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (doctor is null)
        {
            return Result<AppointmentResponse>.BadRequest($"Doctor {request.DoctorId} does not exist.");
        }

        if (!doctor.IsActive)
        {
            return Result<AppointmentResponse>.BadRequest($"Doctor {doctor.Name} is not active and cannot take appointments.");
        }

        if (request.AppointmentDate == default)
        {
            return Result<AppointmentResponse>.BadRequest("AppointmentDate is required.");
        }

        if (request.AppointmentDate < DateTime.UtcNow)
        {
            return Result<AppointmentResponse>.BadRequest("AppointmentDate cannot be in the past.");
        }

        var slotTaken = await _context.Appointments.AnyAsync(a =>
            a.PatientId == patientId &&
            a.DoctorId == request.DoctorId &&
            a.AppointmentDate == request.AppointmentDate &&
            (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed),
            cancellationToken);

        if (slotTaken)
        {
            return Result<AppointmentResponse>.Conflict(
                $"You already have an appointment with Doctor {doctor.Name} at {request.AppointmentDate:yyyy-MM-dd HH:mm}.");
        }

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            AppointmentDate = request.AppointmentDate,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        appointment.Patient = patient;
        appointment.Doctor = doctor;

        return Result<AppointmentResponse>.Success(AppointmentResponse.FromEntity(appointment));
    }
}
