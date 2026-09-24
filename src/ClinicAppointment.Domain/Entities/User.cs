namespace ClinicAppointment.Domain.Entities;

/// <summary>
/// A login account. Only the fields authentication needs are stored here.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password produced by the framework hasher. Plain text is never stored,
    /// and this value is never returned by the API.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    /// <summary>
    /// For a Patient account: the Patient record that owns its appointments.
    /// Null for accounts that are not patients (a Doctor account has no patient record).
    /// </summary>
    public int? PatientId { get; set; }
}
