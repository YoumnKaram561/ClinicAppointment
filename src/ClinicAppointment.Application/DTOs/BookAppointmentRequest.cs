using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.Application.DTOs;

public class BookAppointmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "DoctorId is required.")]
    public int DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }
}
