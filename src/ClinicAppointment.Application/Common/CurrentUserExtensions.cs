using ClinicAppointment.Application.Interfaces;

namespace ClinicAppointment.Application.Common;

/// <summary>
/// Small helper so the patient operations all resolve "who is asking" the same way:
/// from the authenticated token, never from a value sent in the request.
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>
    /// Returns the Patient record of the signed-in account. False means the caller is not a
    /// patient account, so patient-only operations must stop.
    /// </summary>
    public static bool TryGetPatientId(this ICurrentUser currentUser, out int patientId)
    {
        patientId = currentUser.PatientId ?? 0;

        return currentUser.PatientId is > 0;
    }
}
