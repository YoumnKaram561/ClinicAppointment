using ClinicAppointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }

    DbSet<Doctor> Doctors { get; }

    DbSet<Appointment> Appointments { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
