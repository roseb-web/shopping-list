
namespace ShoppingListApp.Core.Results;

public enum ServiceError
{
    None,
    NotFound,
    ValidationError,
    Conflict,
    Error
}

public class ServiceResult
{
    public ServiceError Error { get; set; } = ServiceError.None;
    public string ErrorMessage { get; set; } = string.Empty;

    public bool IsSuccess => Error == ServiceError.None;

    public static ServiceResult Ok()
    {
        return new ServiceResult();
    }
    public static ServiceResult Fail(ServiceError error, string errorMessage)
    {
        return new ServiceResult { Error = error, ErrorMessage = errorMessage };
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; set; }

    public static ServiceResult<T> Ok(T value)
    {
        return new ServiceResult<T> { Value = value };
    }
    public static new ServiceResult<T> Fail(ServiceError error, string errorMessage)
    {
        return new ServiceResult<T> { Error = error, ErrorMessage = errorMessage };
    }
}