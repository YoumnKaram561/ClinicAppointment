using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        // One answer for "no such account" and "wrong password", so the endpoint cannot be
        // used to find out which email addresses are registered.
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return Result<AuthResponse>.Unauthorized("Email or password is incorrect.");
        }

        var (token, expiresAt) = _tokenGenerator.CreateToken(user);

        return Result<AuthResponse>.Success(
            new AuthResponse(token, expiresAt, UserResponse.FromEntity(user)));
    }
}
