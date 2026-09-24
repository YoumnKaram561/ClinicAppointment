namespace ClinicAppointment.Application.DTOs;

/// <summary>
/// Body of a successful POST /api/auth/login. The token must be sent as
/// "Authorization: Bearer {token}" on protected endpoints.
/// </summary>
public record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    UserResponse User);
