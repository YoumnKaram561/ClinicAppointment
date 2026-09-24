using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ClinicAppointment.Infrastructure.Authentication;

public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new User(), password);

    public bool Verify(string storedHash, string providedPassword) =>
        _hasher.VerifyHashedPassword(new User(), storedHash, providedPassword) != PasswordVerificationResult.Failed;
}
