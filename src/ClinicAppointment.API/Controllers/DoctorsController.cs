using ClinicAppointment.API.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Features.Doctors.Queries.GetDoctorById;
using ClinicAppointment.Application.Features.Doctors.Queries.GetDoctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers;

/// <summary>Doctors registered in the clinic.</summary>
/// <remarks>Signing in is required, so the list of doctors is not public; both roles may read it.</remarks>
[ApiController]
[Route("api/doctors")]
[Produces("application/json")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Lists the doctors that currently accept appointments.</summary>
    /// <remarks>Inactive doctors are not returned; use GET /api/doctors/{id} to look one up by id.</remarks>
    /// <response code="200">Active doctors, ordered by name.</response>
    /// <response code="401">No valid token was sent.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctors(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDoctorsQuery(), cancellationToken);

        return this.ToActionResult(result);
    }

    /// <summary>Gets one doctor by id, including doctors that are marked inactive.</summary>
    /// <param name="id">Doctor id, e.g. 1.</param>
    /// <param name="cancellationToken">Aborts the request when the client disconnects.</param>
    /// <response code="200">The doctor.</response>
    /// <response code="401">No valid token was sent.</response>
    /// <response code="404">No doctor has this id.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDoctorByIdQuery(id), cancellationToken);

        return this.ToActionResult(result);
    }
}
