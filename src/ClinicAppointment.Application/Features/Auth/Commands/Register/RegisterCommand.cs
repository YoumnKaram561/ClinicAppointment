using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Domain.Entities;
using MediatR;

namespace ClinicAppointment.Application.Features.Auth.Commands.Register;

/// <summary>
/// Creates a login account. A Patient account also gets the Patient record its
/// appointments will belong to.
/// </summary>
public record RegisterCommand(
    string Name,
    string Email,
    string Password,
    UserRole Role) : IRequest<Result<UserResponse>>;
