using ClinicAppointment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointment.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        // One account per address, and lookups during login run on this column.
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // One account belongs to one patient record (null for a doctor account).
        // Patient has no back reference on purpose, so it stays free of login details.
        builder.HasOne<Patient>()
            .WithOne()
            .HasForeignKey<User>(u => u.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // SQL Server counts NULL values as duplicates in a unique index, so the filter keeps
        // the "one account per patient record" rule without blocking several doctor accounts.
        builder.HasIndex(u => u.PatientId)
            .IsUnique()
            .HasFilter("[PatientId] IS NOT NULL");
    }
}
