
namespace ShoppingListApp.Core.Results;

public enum ServiceError
{
    None,
    NotFound,
    ValidationError,
    Error
}

public class ServiceResult
{
    public ServiceError Error { get; set; } = ServiceError.None;
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; set; }

    public static ServiceResult<T> Ok(T value)
    {
        return new ServiceResult<T> { Value = value };
    }

    public static ServiceResult<T> Fail(ServiceError error, string errorMessage)
    {
        return new ServiceResult<T> { Error = error, ErrorMessage = errorMessage };
    }
}