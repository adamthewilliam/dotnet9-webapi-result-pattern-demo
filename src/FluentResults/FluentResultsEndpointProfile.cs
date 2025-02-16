using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Responses;
using Dotnet9.WebApi.ResultPattern.Demo.Domain;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.WebApi.ResultPattern.Demo.FluentResults;

public class FluentResultsEndpointProfile : DefaultAspNetCoreResultEndpointProfile
{
    private Func<HttpContext>? _httpContextProvider;

    public void SetHttpContextProvider(Func<HttpContext> httpContextProvider)
    {
        _httpContextProvider = httpContextProvider;
    }
    
    public override ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        var result = context.Result;

        if (result.HasError<ValidationError>(out var validationErrors))
        {
            return new BadRequestObjectResult(
                validationErrors.Select(e => new ErrorResponseDto(e.Message, e.ErrorCode)));
        }

        if (result.HasError<NotFoundError>(out var notFoundErrors))
        {
            var notFoundError = notFoundErrors.First();
            
            return new NotFoundObjectResult(
                new ErrorResponseDto(notFoundError.Message, notFoundError.ErrorCode));
        }

        if (result.HasError<ConflictError>(out var conflictErrors))
        {
            var conflictError = conflictErrors.First();
            
            return new ConflictObjectResult(
                new ErrorResponseDto(conflictError.Message, conflictError.ErrorCode));
        }

        if (result.HasError<UnauthorizedError>(out var unauthorizedErrors))
        {
            var unauthorizedError = unauthorizedErrors.First();
            
            return new UnauthorizedObjectResult(
                new ErrorResponseDto(unauthorizedError.Message, unauthorizedError.ErrorCode));
        }

        if (result.HasError<ForbiddenError>(out var forbiddenErrors))
        {
            var forbiddenError = forbiddenErrors.First();
            
            return new ObjectResult(
                new ErrorResponseDto(forbiddenError.Message, forbiddenError.ErrorCode))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        if (result.HasError<ThrottlingError>(out var throttlingErrors))
        {
            var error = throttlingErrors.First();
            
            var response = new ErrorResponseDto(error.Message, error.ErrorCode,
                new Dictionary<string, object> { { "RetryAfter", error.RetryAfter } });

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status429TooManyRequests
            };
        }
        
        if (result.HasError<InternalServerError>(out var serverErrors))
        {
            var error = serverErrors.First();
            return new ObjectResult(
                new ErrorResponseDto(error.Message, error.ErrorCode))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        
        if (result.HasError<DomainError>(out var domainErrors))
        {
            var domainError = domainErrors.First();
            
            return new BadRequestObjectResult(
                new ErrorResponseDto(domainError.Message, domainError.ErrorCode));
        }

        return new ObjectResult(new ErrorResponseDto("An unexpected error occurred", "500"))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    public override ActionResult TransformOkNoValueResultToActionResult(
        OkResultToActionResultTransformationContext<Result> context)
    {
        var httpContext = _httpContextProvider?.Invoke();

        if (httpContext is null) return new OkResult();
        
        if (IsSuccessfulNoContentRequest(httpContext.Request.Method))
        {
            return new NoContentResult();
        }
        
        return new OkResult();
    }

    public override ActionResult TransformOkValueResultToActionResult<T>(
        OkResultToActionResultTransformationContext<Result<T>> context)
    {
        var httpContext = _httpContextProvider?.Invoke();
        
        if (httpContext == null)
        {
            return new OkObjectResult(context.Result.Value);
        }

        if (httpContext.Request.Method == HttpMethods.Post && 
            httpContext.Items["Location"] is string location)
        {
            return new CreatedResult(location, context.Result.Value);
        }
        
        return new OkObjectResult(context.Result.Value);
    }
    
    private static bool IsSuccessfulNoContentRequest(string method)
    {
        return method switch
        {
            "DELETE" => true,
            "PUT" => true,
            "PATCH" => true,
            _ => false
        };
    }
}