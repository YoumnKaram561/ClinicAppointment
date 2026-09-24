namespace ClinicAppointment.Application.Common;

/// <summary>
/// The claim names put in the JWT and read back from it. Defined once so the token
/// creator and the reader cannot disagree. Short names are used because the JWT bearer
/// handler does not translate them, see Program.cs where RoleClaimType/NameClaimType are set.
/// </summary>
public static class AuthTokenClaims
{
    public const string Subject = "sub";
    public const string Name = "name";
    public const string Email = "email";
    public const string Role = "role";

    /// <summary>Patient record of the signed-in account; only present for Patient tokens.</summary>
    public const string PatientId = "patient_id";
}
