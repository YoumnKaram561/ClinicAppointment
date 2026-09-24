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

// A body that fails model validation is reported the same way as a failed business rule,
// so every 400 from this API has one shape: { "error": "message" }.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var messages = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => entry.Value!.Errors.Select(error =>
            {
                // Keys arrive as "request.DoctorId"; the JSON body uses camelCase names.
                var field = entry.Key[(entry.Key.LastIndexOf('.') + 1)..];
                return field.Length == 0
                    ? error.ErrorMessage
                    : $"{char.ToLowerInvariant(field[0])}{field[1..]}: {error.ErrorMessage}";
            }));

        return new BadRequestObjectResult(new ErrorResponse(string.Join(" ", messages)));
    };
});

// The "Jwt" section of appsettings.json holds the secret, issuer, audience and lifetime.
// Nothing here is hardcoded, and Infrastructure reads the same section to sign its tokens.
var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);
jwtSettings.Validate();

// A rejected token or a missing role is reported by middleware, not by a controller, so it has to
// be written out here to keep the one error shape the rest of the API uses: { "error": "message" }.
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
        // Keep the short claim names written by JwtTokenService ("role", "patient_id", ...).
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
            // A token stops working at its own expiry time instead of living a few minutes longer.
            ClockSkew = TimeSpan.Zero,
            NameClaimType = AuthTokenClaims.Name,
            RoleClaimType = AuthTokenClaims.Role,
        };

        options.Events = new JwtBearerEvents
        {
            // No token, a broken token or an expired one.
            OnChallenge = context =>
            {
                context.HandleResponse();

                return WriteAuthError(
                    context.Response,
                    StatusCodes.Status401Unauthorized,
                    "A valid bearer token is required. Sign in through POST /api/auth/login.",
                    "Bearer");
            },

            // A valid token whose role is not allowed for the endpoint.
            OnForbidden = context => WriteAuthError(
                context.Response,
                StatusCodes.Status403Forbidden,
                "This account is not allowed to perform this operation.")
        };
    });

builder.Services.AddAuthorization();

// Who is calling is read from the validated token, so handlers never ask the request for an id.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, JwtCurrentUser>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Controller comments document the endpoints, Application comments document the DTO schemas.
    foreach (var xmlFile in new[] { "ClinicAppointment.API.xml", "ClinicAppointment.Application.xml" })
    {
        options.IncludeXmlComments(
            Path.Combine(AppContext.BaseDirectory, xmlFile),
            includeControllerXmlComments: true);
    }

    // Adds the Authorize button, so protected endpoints can be tested from Swagger UI.
    // Type Http + scheme "bearer" means you paste only the token; "Bearer " is added for you.
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

// Authentication first: it fills HttpContext.User, which authorization then checks.
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
