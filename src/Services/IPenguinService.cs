using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Requests;
using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Responses;
using FluentResults;

namespace Dotnet9.WebApi.ResultPattern.Demo.Services;

public interface IPenguinService
{
    Task<Result<PenguinResponseDto>> GetPenguinByIdAsync(Guid penguinId, CancellationToken ct = default);
    
    Task<Result<IReadOnlyList<PenguinResponseDto>>> GetPenguinsAsync(CancellationToken ct = default);
    
    Task<Result<PenguinResponseDto>> CreatePenguinAsync(CreatePenguinRequestDto request);
}