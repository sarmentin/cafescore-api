using Cafescore.Application.DTOs.Clinica;
using Cafescore.Domain.Interfaces;

namespace Cafescore.Application.Services;

public class ClinicaService
{
    private readonly IClinicaRepository _clinicaRepository;

    public ClinicaService(IClinicaRepository clinicaRepository)
    {
        _clinicaRepository = clinicaRepository;
    }

    public async Task<IEnumerable<ClinicaDto>> ObterTodasAsync()
    {
        var clinicas = await _clinicaRepository.ObterTodasAsync();

        return clinicas.Select(c => new ClinicaDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Endereco = c.Endereco,
            Cidade = c.Cidade,
            NotaMedia = c.Avaliacoes.Any()
                ? Math.Round(c.Avaliacoes.Average(a => a.Nota), 1)
                : 0,
            TotalAvaliacoes = c.Avaliacoes.Count
        });
    }

    public async Task<ClinicaDto> ObterPorIdAsync(Guid id)
    {
        var clinica = await _clinicaRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Clínica não encontrada");

        return new ClinicaDto
        {
            Id = clinica.Id,
            Nome = clinica.Nome,
            Endereco = clinica.Endereco,
            Cidade = clinica.Cidade,
            NotaMedia = clinica.Avaliacoes.Any()
                ? Math.Round(clinica.Avaliacoes.Average(a => a.Nota), 1)
                : 0,
            TotalAvaliacoes = clinica.Avaliacoes.Count
        };
    }
}