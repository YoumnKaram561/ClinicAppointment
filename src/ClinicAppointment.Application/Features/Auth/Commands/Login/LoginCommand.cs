using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using MediatR;

namespace ClinicAppointment.Application.Features.Auth.Commands.Login;

/// <summary>Checks the credentials and returns a signed JWT for the account.</summary>
public record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;
