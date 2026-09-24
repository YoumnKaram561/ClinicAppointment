using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.Application.DTOs;

/// <summary>
/// Body of POST /api/appointments. The patient is not asked for: the appointment is
/// always booked for the account in the bearer token.
/// </summary>
public class BookAppointmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "DoctorId is required.")]
    public int DoctorId { get; set; }

    /// <summary>
    /// Must be a future date and time. Checked by the booking handler,
    /// because an omitted value arrives as DateTime.MinValue.
    /// </summary>
    public DateTime AppointmentDate { get; set; }
}
