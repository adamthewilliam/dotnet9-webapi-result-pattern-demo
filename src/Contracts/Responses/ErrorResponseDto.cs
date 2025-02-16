namespace Dotnet9.WebApi.ResultPattern.Demo.Contracts.Responses;

public record ErrorResponseDto(string Message, string ErrorCode, IDictionary<string, object>? Metadata = null);