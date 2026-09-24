using ClinicAppointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Interfaces;

/// <summary>
/// The data access contract the handlers use. It is implemented by the
/// Infrastructure layer, so Application stays free of EF Core configuration details.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }

    DbSet<Doctor> Doctors { get; }

    DbSet<Appointment> Appointments { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
