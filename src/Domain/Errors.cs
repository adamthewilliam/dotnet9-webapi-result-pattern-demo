using FluentResults;

namespace Dotnet9.WebApi.ResultPattern.Demo.Domain;

public abstract class DomainError : Error
{
    public string ErrorCode { get; }

    protected DomainError(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class ValidationError : DomainError
{
    public string PropertyName { get; }

    public ValidationError(string propertyName, string message)
        : base($"Validation failed for '{propertyName}': {message}", "422")
    {
        PropertyName = propertyName;
    }
}

public class NotFoundError : DomainError
{
    public string EntityName { get; }
    public object Id { get; }

    public NotFoundError(string entityName, object id)
        : base($"'{entityName}' with id '{id}' not found.", "404")
    {
        EntityName = entityName;
        Id = id;
    }
}

public class ConflictError : DomainError
{
    public string EntityName { get; }

    public ConflictError(string entityName, string message)
        : base($"Conflict occurred with '{entityName}': {message}", "409")
    {
        EntityName = entityName;
    }
}

public class UnauthorizedError : DomainError
{
    public UnauthorizedError(string message)
        : base(message, "401")
    {
    }
}

public class ForbiddenError : DomainError
{
    public string Resource { get; }

    public ForbiddenError(string resource, string message)
        : base($"Access forbidden to {resource}: {message}", "403")
    {
        Resource = resource;
    }
}

public class ThrottlingError : DomainError
{
    public DateTime RetryAfter { get; }

    public ThrottlingError(DateTime retryAfter)
        : base($"Too many requests. Please retry after {retryAfter}", "429")
    {
        RetryAfter = retryAfter;
    }
}

public class InternalServerError : DomainError
{
    public InternalServerError(string message, string? details = null, string? errorCode = null)
        : base(message, "500")
    {
    }
}