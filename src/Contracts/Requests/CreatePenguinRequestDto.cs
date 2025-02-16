namespace Dotnet9.WebApi.ResultPattern.Demo.Contracts.Requests;

public record CreatePenguinRequestDto(Guid Id, string Name, string Species, int Age);