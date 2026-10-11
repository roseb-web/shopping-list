using ShoppingListApp.Core.Results;

namespace ShoppingListApp.Api.Endpoints;

public static class ServiceResultExtensions
{
    // Success with a value: 200 OK with the value as the body
    public static IResult ToHttpResult<T>(this ServiceResult<T> result)
    {
        return result.IsSuccess ? Results.Ok(result.Value) : ToProblem(result);
    }

    // Success with nothing to return: 204 No Content
    public static IResult ToHttpResult(this ServiceResult result)
    {
        return result.IsSuccess ? Results.NoContent() : ToProblem(result);
    }

    // Failure: pick the status code that matches the error
    private static IResult ToProblem(ServiceResult result)
    {
        var statusCode = result.Error switch
        {
            ServiceError.NotFound => StatusCodes.Status404NotFound,
            ServiceError.ValidationError => StatusCodes.Status400BadRequest,
            ServiceError.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(detail: result.ErrorMessage, statusCode: statusCode);
    }
}
