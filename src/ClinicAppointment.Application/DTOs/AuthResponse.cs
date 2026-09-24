namespace ClinicAppointment.Application.DTOs;

public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    UserResponse User);
