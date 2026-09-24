namespace ClinicAppointment.Application.Interfaces;

/// <summary>
/// Password hashing, implemented in Infrastructure with the ASP.NET Core hasher.
/// The Application layer only says what it needs, not which algorithm is used.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string storedHash, string providedPassword);
}
