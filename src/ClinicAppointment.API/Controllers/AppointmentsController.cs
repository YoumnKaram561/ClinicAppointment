using ClinicAppointment.API.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Features.Appointments.Commands.BookAppointment;
using ClinicAppointment.Application.Features.Appointments.Commands.CancelAppointment;
using ClinicAppointment.Application.Features.Appointments.Commands.CompleteAppointment;
using ClinicAppointment.Application.Features.Appointments.Commands.ConfirmAppointment;
using ClinicAppointment.Application.Features.Appointments.Commands.RejectAppointment;
using ClinicAppointment.Application.Features.Appointments.Queries.GetAppointmentById;
using ClinicAppointment.Application.Features.Appointments.Queries.GetMyAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers;

[ApiController]
[Route("api/appointments")]
[Produces("application/json")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Book(
        [FromBody] BookAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new BookAppointmentCommand(
            request.DoctorId,
            request.AppointmentDate);

        var result = await _mediator.Send(command, cancellationToken);

        return this.ToCreatedActionResult(result, nameof(GetAppointmentById), new { id = result.Value?.Id });
    }

    [HttpGet("my")]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyAppointments(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyAppointmentsQuery(), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:int}", Name = nameof(GetAppointmentById))]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(id), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:int}/confirm")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ConfirmAppointmentCommand(id), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:int}/reject")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectAppointmentCommand(id), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelAppointmentCommand(id), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:int}/complete")]
    [Authorize(Roles = "Doctor")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteAppointmentCommand(id), cancellationToken);

        return this.ToActionResult(result);
    }
}
