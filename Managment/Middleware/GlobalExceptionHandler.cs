using Management.Middleware.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Management.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = new();

            switch (exception)
            {
                case UserAlreadyExistsException:
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Title = exception.Message;
                    break;
                case NoSuchUserException:
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = exception.Message;
                    break;
                case UserCreationException:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = exception.Message;
                    break;
                default:
                    return false;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
