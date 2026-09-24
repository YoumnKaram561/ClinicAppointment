using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var name = request.Name.Trim();

        if (!Enum.IsDefined(request.Role))
        {
            return Result<UserResponse>.BadRequest("Role must be Patient or Doctor.");
        }

        // Email addresses are compared as stored; the database collation is case insensitive.
        var emailTaken = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

        if (emailTaken)
        {
            return Result<UserResponse>.Conflict($"An account with {email} already exists.");
        }

        var user = new User
        {
            Name = name,
            Email = email,
            // Only the hash is stored; the plain password never reaches the database.
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = request.Role
        };

        if (user.Role == UserRole.Patient)
        {
            // Appointments belong to a Patient record, so a patient account is given one.
            // The phone number is not part of registration and can be filled in later.
            var patient = new Patient { Name = name, Email = email, Phone = string.Empty };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);

            user.PatientId = patient.Id;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<UserResponse>.Success(UserResponse.FromEntity(user));
    }
}
