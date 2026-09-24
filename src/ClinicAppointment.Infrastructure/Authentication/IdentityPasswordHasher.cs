using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ClinicAppointment.Infrastructure.Authentication;

/// <summary>
/// Uses the password hasher that ships with ASP.NET Core Identity (PBKDF2, HMACSHA256,
/// per-password salt, iterating work factor). No hashing algorithm is implemented here.
/// Only the hasher part of Identity is used - no users, sign-in managers or cookies.
/// </summary>
public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new User(), password);

    public bool Verify(string storedHash, string providedPassword) =>
        _hasher.VerifyHashedPassword(new User(), storedHash, providedPassword) != PasswordVerificationResult.Failed;
}
