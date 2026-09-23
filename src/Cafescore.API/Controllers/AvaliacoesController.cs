using System.Security.Claims;
using Cafescore.Application.DTOs.Avaliacao;
using Cafescore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cafescore.API.Controllers;

[ApiController]
[Route("api/avaliacoes")]
[EnableRateLimiting("geral")]
public class AvaliacoesController : ControllerBase
{
    private readonly AvaliacaoService _avaliacaoService;

    public AvaliacoesController(AvaliacaoService avaliacaoService)
    {
        _avaliacaoService = avaliacaoService;
    }

    private Guid UsuarioIdLogado =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("minhas")]
    [Authorize]
    public async Task<IActionResult> ObterMinhas()
    {
        var avaliacoes = await _avaliacaoService.ObterPorUsuarioAsync(UsuarioIdLogado);
        return Ok(avaliacoes);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] CriarAvaliacaoDto dto)
    {
        var avaliacao = await _avaliacaoService.CriarAsync(dto, UsuarioIdLogado);
        return CreatedAtAction(nameof(ObterMinhas), avaliacao);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarAvaliacaoDto dto)
    {
        await _avaliacaoService.AtualizarAsync(id, dto, UsuarioIdLogado);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Remover(Guid id)
    {
        await _avaliacaoService.RemoverAsync(id, UsuarioIdLogado);
        return NoContent();
    }
}