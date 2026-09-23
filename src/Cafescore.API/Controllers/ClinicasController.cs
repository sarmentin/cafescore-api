using Cafescore.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cafescore.API.Controllers;

[ApiController]
[Route("api/clinicas")]
[EnableRateLimiting("geral")]
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
        var clinica = await _clinicaService.ObterPorIdAsync(id);
        return Ok(clinica);
    }

    [HttpGet("{id}/avaliacoes")]
    public async Task<IActionResult> ObterAvaliacoes(Guid id)
    {
        var avaliacoes = await _avaliacaoService.ObterPorClinicaAsync(id);
        return Ok(avaliacoes);
    }
}