using ClinicAppointment.Domain.Entities;

namespace ClinicAppointment.Application.Interfaces;

public interface ITokenGenerator
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
