namespace ClinicAppointment.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string storedHash, string providedPassword);
}
