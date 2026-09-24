using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Domain.Entities;
using MediatR;

namespace ClinicAppointment.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Name,
    string Email,
    string Password,
    UserRole Role) : IRequest<Result<UserResponse>>;
