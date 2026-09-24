using ClinicAppointment.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Common;

/// <summary>
/// Translates the outcome of a Command or Query into an HTTP response,
/// so the controllers do not have to inspect results rule by rule.
/// Success keeps the response body; failures are returned as <see cref="ErrorResponse"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>200 when the operation succeeded, otherwise 400 / 401 / 403 / 404 / 409 with the handler's message.</summary>
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        // A failed Result always carries a message; the failure factories enforce that.
        var error = new ErrorResponse(result.Error!);

        return result.Status switch
        {
            ResultStatus.Unauthorized => controller.Unauthorized(error),
            // 403 keeps the handler's message; Forbid() would ask an authentication handler to build the response.
            ResultStatus.Forbidden => new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden },
            ResultStatus.NotFound => controller.NotFound(error),
            ResultStatus.Conflict => controller.Conflict(error),
            _ => controller.BadRequest(error)
        };
    }

    /// <summary>
    /// 201 for a freshly created account. There is no GET endpoint that returns one account by id,
    /// so no Location header is built here.
    /// </summary>
    public static IActionResult ToCreatedActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        return controller.StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>201 + Location for a created appointment, other status codes as in <see cref="ToActionResult"/>.</summary>
    public static IActionResult ToCreatedActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        string actionName,
        object? routeValues)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        return controller.CreatedAtAction(actionName, routeValues, result.Value);
    }
}
