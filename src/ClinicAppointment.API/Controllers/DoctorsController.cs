using ClinicAppointment.API.Common;
using ClinicAppointment.Application.DTOs;
using ClinicAppointment.Application.Features.Doctors.Queries.GetDoctorById;
using ClinicAppointment.Application.Features.Doctors.Queries.GetDoctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Controllers;

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

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctors(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDoctorsQuery(), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoctorById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDoctorByIdQuery(id), cancellationToken);

        return this.ToActionResult(result);
    }
}
