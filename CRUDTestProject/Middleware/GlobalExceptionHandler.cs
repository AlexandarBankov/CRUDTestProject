﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDTestProject.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            
            switch (exception)
            {
                case ArgumentNullException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = exception.Message;
                    break;
                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Unexpected error";
                    break;
            }

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
