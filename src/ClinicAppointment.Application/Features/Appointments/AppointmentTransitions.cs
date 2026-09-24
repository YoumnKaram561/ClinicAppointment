using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.Features.Appointments;

/// <summary>
/// The one place that decides which appointment status changes are legal, so the four
/// status Commands cannot drift apart. Changing a status is always an update of the
/// Appointment row — an appointment is never deleted.
/// </summary>
internal static class AppointmentTransitions
{
    /// <summary>
    /// Pending → Confirmed / Rejected / Cancelled, Confirmed → Completed / Cancelled.
    /// Everything else (including Completed, Cancelled and Rejected as a source) is illegal,
    /// so Pending → Completed is rejected too.
    /// </summary>
    public static bool IsAllowed(AppointmentStatus from, AppointmentStatus to) => (from, to) switch
    {
        (AppointmentStatus.Pending, AppointmentStatus.Confirmed) => true,
        (AppointmentStatus.Pending, AppointmentStatus.Rejected) => true,
        (AppointmentStatus.Pending, AppointmentStatus.Cancelled) => true,
        (AppointmentStatus.Confirmed, AppointmentStatus.Completed) => true,
        (AppointmentStatus.Confirmed, AppointmentStatus.Cancelled) => true,
        _ => false,
    };

    /// <summary>Message explaining why a transition was refused, e.g. for a 400 response.</summary>
    public static string ErrorFor(int appointmentId, AppointmentStatus from, AppointmentStatus to) =>
        $"Appointment {appointmentId} is {from} and cannot be changed to {to}. {RequirementFor(to)}";

    private static string RequirementFor(AppointmentStatus to) => to switch
    {
        AppointmentStatus.Confirmed => "Only a Pending appointment can be confirmed.",
        AppointmentStatus.Rejected => "Only a Pending appointment can be rejected.",
        AppointmentStatus.Completed => "Only a Confirmed appointment can be completed.",
        AppointmentStatus.Cancelled => "Only a Pending or Confirmed appointment can be cancelled.",
        _ => "This status change is not supported.",
    };
}
