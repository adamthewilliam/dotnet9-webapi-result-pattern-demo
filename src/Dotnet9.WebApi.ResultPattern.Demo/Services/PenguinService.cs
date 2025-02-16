using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Requests;
using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Responses;
using Dotnet9.WebApi.ResultPattern.Demo.Data;
using Dotnet9.WebApi.ResultPattern.Demo.Data.Models;
using Dotnet9.WebApi.ResultPattern.Demo.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Dotnet9.WebApi.ResultPattern.Demo.Services;

public class PenguinService : IPenguinService
{
    private readonly ApplicationDbContext _dbContext;
    
    public PenguinService(ApplicationDbContext dbContext)
    {
       _dbContext = dbContext;
    }
    
    public async Task<Result<PenguinResponseDto>> GetPenguinByIdAsync(
        Guid penguinId,
        CancellationToken ct = default)
    {
        var penguin = await _dbContext.Penguins
            .AsNoTracking()
            .Where(p => p.Id == penguinId)
            .Select(p => new PenguinResponseDto(p.Id, p.Name, p.Species, p.Age))
            .FirstOrDefaultAsync(ct);

        if (penguin is null)
            return Result.Fail(new NotFoundError(nameof(PenguinModel), penguinId));

        return Result.Ok(penguin);
    }

    public async Task<Result<IReadOnlyList<PenguinResponseDto>>> GetPenguinsAsync(
        CancellationToken ct = default)
    {
        var penguins = await _dbContext.Penguins.AsNoTracking()
            .Select(x => new PenguinResponseDto(x.Id, x.Name, x.Species, x.Age))
            .ToListAsync(ct);
        
        return Result.Ok<IReadOnlyList<PenguinResponseDto>>(penguins);
    }

    public async Task<Result<PenguinResponseDto>> CreatePenguinAsync(
        CreatePenguinRequestDto request)
    {
        var newPenguin = new PenguinModel
        {
            Name = request.Name,
            Species = request.Species,
            Age = request.Age
        };
        
        _dbContext.Penguins.Add(newPenguin);
        
        var insertedEntries = await _dbContext.SaveChangesAsync();

        if (insertedEntries <= 0)
            return Result.Fail(new InternalServerError("An Unexpected error occurred whilst creating a Penguin"));
        
        var result = new PenguinResponseDto(newPenguin.Id, newPenguin.Name, newPenguin.Species, newPenguin.Age);
        return Result.Ok(result);
    }
}