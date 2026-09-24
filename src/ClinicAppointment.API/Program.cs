using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClinicAppointment.API.Common;
using ClinicAppointment.API.Middleware;
using ClinicAppointment.Application;
using ClinicAppointment.Application.Common;
using ClinicAppointment.Application.Interfaces;
using ClinicAppointment.Infrastructure;
using ClinicAppointment.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var messages = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => entry.Value!.Errors.Select(error =>
            {

                var field = entry.Key[(entry.Key.LastIndexOf('.') + 1)..];
                return field.Length == 0
                    ? error.ErrorMessage
                    : $"{char.ToLowerInvariant(field[0])}{field[1..]}: {error.ErrorMessage}";
            }));

        return new BadRequestObjectResult(new ErrorResponse(string.Join(" ", messages)));
    };
});

var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);
jwtSettings.Validate();

var errorJsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

Task WriteAuthError(HttpResponse response, int statusCode, string message, string? wwwAuthenticate = null)
{
    response.StatusCode = statusCode;
    response.ContentType = "application/json";

    if (wwwAuthenticate is not null)
    {
        response.Headers.WWWAuthenticate = wwwAuthenticate;
    }

    return response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(message), errorJsonOptions));
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero,
            NameClaimType = AuthTokenClaims.Name,
            RoleClaimType = AuthTokenClaims.Role,
        };

        options.Events = new JwtBearerEvents
        {

            OnChallenge = context =>
            {
                context.HandleResponse();

                return WriteAuthError(
                    context.Response,
                    StatusCodes.Status401Unauthorized,
                    "A valid bearer token is required. Sign in through POST /api/auth/login.",
                    "Bearer");
            },

            OnForbidden = context => WriteAuthError(
                context.Response,
                StatusCodes.Status403Forbidden,
                "This account is not allowed to perform this operation.")
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, JwtCurrentUser>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    foreach (var xmlFile in new[] { "ClinicAppointment.API.xml", "ClinicAppointment.Application.xml" })
    {
        options.IncludeXmlComments(
            Path.Combine(AppContext.BaseDirectory, xmlFile),
            includeControllerXmlComments: true);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste the token from POST /api/auth/login here (without the word Bearer).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
