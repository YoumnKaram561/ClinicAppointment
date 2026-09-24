namespace ClinicAppointment.Application.Common;

public enum ResultStatus
{
    Success,
    BadRequest,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict
}

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

    public static Result<T> Unauthorized(string error) => Fail(ResultStatus.Unauthorized, error);

    public static Result<T> Forbidden(string error) => Fail(ResultStatus.Forbidden, error);

    public static Result<T> NotFound(string error) => Fail(ResultStatus.NotFound, error);

    public static Result<T> Conflict(string error) => Fail(ResultStatus.Conflict, error);

    private static Result<T> Fail(ResultStatus status, string error) => new(false, status, default, error);
}
