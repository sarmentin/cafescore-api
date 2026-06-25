using System.Security.Claims;
using Cafescore.Application.DTOs.Avaliacao;
using Cafescore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafescore.API.Controllers;

[ApiController]
[Route("api/avaliacoes")]
public class AvaliacoesController : ControllerBase
{
    private readonly AvaliacaoService _avaliacaoService;

    public AvaliacoesController(AvaliacaoService avaliacaoService)
    {
        _avaliacaoService = avaliacaoService;
    }

    [HttpGet("minhas")]
    [Authorize]
    public async Task<IActionResult> ObterMinhas()
    {
        var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var avaliacoes = await _avaliacaoService.ObterPorUsuarioAsync(usuarioId);
        return Ok(avaliacoes);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Criar([FromBody] CriarAvaliacaoDto dto)
    {
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var avaliacao = await _avaliacaoService.CriarAsync(dto, usuarioId);
            return CreatedAtAction(nameof(ObterMinhas), avaliacao);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarAvaliacaoDto dto)
    {
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _avaliacaoService.AtualizarAsync(id, dto, usuarioId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Remover(Guid id)
    {
        try
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _avaliacaoService.RemoverAsync(id, usuarioId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}