using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Requests;
using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Responses;
using Dotnet9.WebApi.ResultPattern.Demo.Services;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.WebApi.ResultPattern.Demo.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class PenguinsController : ControllerBase
{
    private readonly IPenguinService _penguinService;

    public PenguinsController(IPenguinService penguinService)
    {
        _penguinService = penguinService;
    }
    
    [HttpGet("{id:guid:required}", Name = nameof(GetPenguinById))]
    [ProducesResponseType(typeof(PenguinResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPenguinById(Guid id, CancellationToken ct = default)
    {
        return await _penguinService
            .GetPenguinByIdAsync(id, ct)
            .ToActionResult();
    }
    
    [HttpGet(Name = nameof(GetPenguins))]
    [ProducesResponseType(typeof(IReadOnlyList<PenguinResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPenguins(CancellationToken ct = default)
    {
        return await _penguinService
            .GetPenguinsAsync(ct)
            .ToActionResult();
    }
    
    [HttpPost(Name = nameof(CreatePenguin))]
    [ProducesResponseType(typeof(PenguinResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePenguin([FromBody] CreatePenguinRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return await _penguinService
            .CreatePenguinAsync(request)
            .ToActionResult();
    }
}