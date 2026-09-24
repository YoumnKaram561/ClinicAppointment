namespace ClinicAppointment.API.Common;

/// <summary>
/// Body of every error the API returns on purpose: <c>{ "error": "message" }</c>.
/// Model-binding failures keep the framework's ProblemDetails shape instead.
/// </summary>
public record ErrorResponse(string Error);
