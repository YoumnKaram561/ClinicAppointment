using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.Features.Appointments;

internal static class AppointmentTransitions
{

    public static bool IsAllowed(AppointmentStatus from, AppointmentStatus to) => (from, to) switch
    {
        (AppointmentStatus.Pending, AppointmentStatus.Confirmed) => true,
        (AppointmentStatus.Pending, AppointmentStatus.Rejected) => true,
        (AppointmentStatus.Pending, AppointmentStatus.Cancelled) => true,
        (AppointmentStatus.Confirmed, AppointmentStatus.Completed) => true,
        (AppointmentStatus.Confirmed, AppointmentStatus.Cancelled) => true,
        _ => false,
    };

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
