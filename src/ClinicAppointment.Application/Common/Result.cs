namespace ClinicAppointment.Application.Common;

/// <summary>
/// Describes why an operation failed. The API layer maps these values to HTTP status codes.
/// </summary>
public enum ResultStatus
{
    Success,
    BadRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict
}

/// <summary>
/// Simple outcome wrapper for Commands and Queries, so business errors are returned
/// as data instead of thrown as exceptions.
/// </summary>
public class Result<T>
{
    private Result(bool isSuccess, ResultStatus status, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Status = status;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public ResultStatus Status { get; }

    public T? Value { get; }

    public string? Error { get; }

    public static Result<T> Success(T value) => new(true, ResultStatus.Success, value, null);

    public static Result<T> BadRequest(string error) => Fail(ResultStatus.BadRequest, error);

    /// <summary>The caller is not signed in (or its credentials did not check out).</summary>
    public static Result<T> Unauthorized(string error) => Fail(ResultStatus.Unauthorized, error);

    /// <summary>The caller is signed in but not allowed to touch this record.</summary>
    public static Result<T> Forbidden(string error) => Fail(ResultStatus.Forbidden, error);

    public static Result<T> NotFound(string error) => Fail(ResultStatus.NotFound, error);

    public static Result<T> Conflict(string error) => Fail(ResultStatus.Conflict, error);

    private static Result<T> Fail(ResultStatus status, string error) => new(false, status, default, error);
}
