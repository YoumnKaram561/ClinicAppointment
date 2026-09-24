using ClinicAppointment.Application.Interfaces;

namespace ClinicAppointment.Application.Common;

public static class CurrentUserExtensions
{

    public static bool TryGetPatientId(this ICurrentUser currentUser, out int patientId)
    {
        patientId = currentUser.PatientId ?? 0;

        return currentUser.PatientId is > 0;
    }
}
