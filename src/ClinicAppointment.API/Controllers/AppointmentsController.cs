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

/// <summary>
/// Appointment booking and status handling.
/// Status changes are always an update - an appointment is never deleted.
/// </summary>
/// <remarks>
/// Every endpoint here needs a JWT from POST /api/auth/login: a request without a valid token gets
/// 401, and a token whose role is not allowed for the endpoint gets 403. The patient or doctor id is
/// never sent by the client - it comes from the token.
/// </remarks>
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

    /// <summary>Books an appointment for the signed-in patient with a doctor at an exact date and time.</summary>
    /// <remarks>
    /// The appointment is created as Pending and belongs to the patient of the token, so the body has
    /// no patient id. Booking is refused when the doctor does not exist or is inactive, the date is
    /// missing or in the past, or the patient already has a Pending/Confirmed appointment for that
    /// doctor and time.
    /// </remarks>
    /// <param name="request">Doctor id and appointment date/time (UTC).</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="201">Appointment created; the response carries the appointment and Location.</response>
    /// <response code="400">Invalid input, or a booking rule was violated.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The token belongs to a Doctor account, which cannot book.</response>
    /// <response code="409">The patient already has an appointment for that doctor and time.</response>
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

    /// <summary>Lists the appointments of the signed-in patient.</summary>
    /// <remarks>
    /// No patient id is passed: the patient is taken from the token, so one account can never read
    /// another patient's appointments.
    /// </remarks>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The patient's appointments, newest booking first. Empty when the patient has none.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The token belongs to a Doctor account, which has no "my" appointments.</response>
    [HttpGet("my")]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyAppointments(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyAppointmentsQuery(), cancellationToken);

        return this.ToActionResult(result);
    }

    /// <summary>Gets one appointment, including the patient and doctor details it was booked with.</summary>
    /// <remarks>A patient may only open their own appointment; a Doctor account may open any of them to work on it.</remarks>
    /// <param name="id">Appointment id.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The appointment.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The appointment belongs to another patient.</response>
    /// <response code="404">No appointment has this id.</response>
    [HttpGet("{id:int}", Name = nameof(GetAppointmentById))]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(id), cancellationToken);

        return this.ToActionResult(result);
    }

    /// <summary>Confirms a Pending appointment (the doctor accepted it).</summary>
    /// <param name="id">Appointment id.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The appointment is now Confirmed.</response>
    /// <response code="400">The appointment is not Pending, so it cannot be confirmed.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The token belongs to a Patient account; confirming is a doctor action.</response>
    /// <response code="404">No appointment has this id.</response>
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

    /// <summary>Rejects a Pending appointment (the clinic declines the request). The record is kept.</summary>
    /// <param name="id">Appointment id.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The appointment is now Rejected.</response>
    /// <response code="400">The appointment is not Pending, so it cannot be rejected.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The token belongs to a Patient account; rejecting is a doctor action.</response>
    /// <response code="404">No appointment has this id.</response>
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

    /// <summary>Cancels a Pending or Confirmed appointment. Cancelling never deletes the record.</summary>
    /// <remarks>Only the patient the appointment belongs to can cancel it.</remarks>
    /// <param name="id">Appointment id.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The appointment is now Cancelled.</response>
    /// <response code="400">The appointment is Completed, Rejected or already Cancelled.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The appointment belongs to another patient, or the token is a Doctor account.</response>
    /// <response code="404">No appointment has this id.</response>
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

    /// <summary>Marks a Confirmed appointment as visited.</summary>
    /// <param name="id">Appointment id.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The appointment is now Completed.</response>
    /// <response code="400">The appointment is not Confirmed (a Pending appointment must be confirmed first).</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="403">The token belongs to a Patient account; completing is a doctor action.</response>
    /// <response code="404">No appointment has this id.</response>
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
