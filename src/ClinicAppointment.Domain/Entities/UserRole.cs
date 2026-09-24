namespace ClinicAppointment.Domain.Entities;

/// <summary>
/// The two kinds of account this system has. Kept separate from the Doctor entity:
/// a Doctor is a clinic professional with a schedule, a User is a login account.
/// </summary>
public enum UserRole
{
    Patient,
    Doctor
}
