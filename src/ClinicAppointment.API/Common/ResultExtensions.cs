using ClinicAppointment.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.API.Common;

public static class ResultExtensions
{

    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        var error = new ErrorResponse(result.Error!);

        return result.Status switch
        {
            ResultStatus.Unauthorized => controller.Unauthorized(error),

            ResultStatus.Forbidden => new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden },
            ResultStatus.NotFound => controller.NotFound(error),
            ResultStatus.Conflict => controller.Conflict(error),
            _ => controller.BadRequest(error)
        };
    }

    public static IActionResult ToCreatedActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        return controller.StatusCode(StatusCodes.Status201Created, result.Value);
    }

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
