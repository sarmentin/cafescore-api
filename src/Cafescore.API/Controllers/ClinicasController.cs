using Cafescore.Application.DTOs.Avaliacao;
using Cafescore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cafescore.API.Controllers;

[ApiController]
[Route("api/clinicas")]
public class ClinicasController : ControllerBase
{
    private readonly ClinicaService _clinicaService;
    private readonly AvaliacaoService _avaliacaoService;

    public ClinicasController(ClinicaService clinicaService, AvaliacaoService avaliacaoService)
    {
        _clinicaService = clinicaService;
        _avaliacaoService = avaliacaoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var clinicas = await _clinicaService.ObterTodasAsync();
        return Ok(clinicas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            var clinica = await _clinicaService.ObterPorIdAsync(id);
            return Ok(clinica);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{id}/avaliacoes")]
    public async Task<IActionResult> ObterAvaliacoes(Guid id)
    {
        var avaliacoes = await _avaliacaoService.ObterPorClinicaAsync(id);
        return Ok(avaliacoes);
    }
}