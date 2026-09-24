using System.ComponentModel.DataAnnotations;
using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid address.")]
    [StringLength(150, ErrorMessage = "Email cannot be longer than 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be Patient or Doctor.")]
    public UserRole? Role { get; set; }
}
