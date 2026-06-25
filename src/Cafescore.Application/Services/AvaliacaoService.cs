using Cafescore.Application.DTOs.Avaliacao;
using Cafescore.Domain.Entities;
using Cafescore.Domain.Interfaces;

namespace Cafescore.Application.Services;

public class AvaliacaoService
{
    private readonly IAvaliacaoRepository _avaliacaoRepository;
    private readonly IClinicaRepository _clinicaRepository;

    public AvaliacaoService(
        IAvaliacaoRepository avaliacaoRepository,
        IClinicaRepository clinicaRepository)
    {
        _avaliacaoRepository = avaliacaoRepository;
        _clinicaRepository = clinicaRepository;
    }

    public async Task<IEnumerable<AvaliacaoDto>> ObterPorClinicaAsync(Guid clinicaId)
    {
        var avaliacoes = await _avaliacaoRepository.ObterPorClinicaAsync(clinicaId);

        return avaliacoes.Select(a => new AvaliacaoDto
        {
            Id = a.Id,
            ClinicaId = a.ClinicaId,
            NomeClinica = a.Clinica.Nome,
            NomeUsuario = a.Usuario.Nome,
            Nota = a.Nota,
            Comentario = a.Comentario,
            DataCriacao = a.DataCriacao
        });
    }

    public async Task<IEnumerable<AvaliacaoDto>> ObterPorUsuarioAsync(Guid usuarioId)
    {
        var avaliacoes = await _avaliacaoRepository.ObterPorUsuarioAsync(usuarioId);

        return avaliacoes.Select(a => new AvaliacaoDto
        {
            Id = a.Id,
            ClinicaId = a.ClinicaId,
            NomeClinica = a.Clinica.Nome,
            NomeUsuario = a.Usuario.Nome,
            Nota = a.Nota,
            Comentario = a.Comentario,
            DataCriacao = a.DataCriacao
        });
    }

    public async Task<AvaliacaoDto> CriarAsync(CriarAvaliacaoDto dto, Guid usuarioId)
    {
        var clinica = await _clinicaRepository.ObterPorIdAsync(dto.ClinicaId)
            ?? throw new Exception("Clínica não encontrada");

        var avaliacao = new Avaliacao(usuarioId, dto.ClinicaId, dto.Nota, dto.Comentario);

        await _avaliacaoRepository.AdicionarAsync(avaliacao);

        return new AvaliacaoDto
        {
            Id = avaliacao.Id,
            ClinicaId = avaliacao.ClinicaId,
            NomeClinica = clinica.Nome,
            NomeUsuario = string.Empty,
            Nota = avaliacao.Nota,
            Comentario = avaliacao.Comentario,
            DataCriacao = avaliacao.DataCriacao
        };
    }

    public async Task AtualizarAsync(Guid id, AtualizarAvaliacaoDto dto, Guid usuarioId)
    {
        var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Avaliação não encontrada");

        if (avaliacao.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Você não tem permissão para editar esta avaliação");

        avaliacao.Atualizar(dto.Nota, dto.Comentario);
        await _avaliacaoRepository.AtualizarAsync(avaliacao);
    }

    public async Task RemoverAsync(Guid id, Guid usuarioId)
    {
        var avaliacao = await _avaliacaoRepository.ObterPorIdAsync(id)
            ?? throw new Exception("Avaliação não encontrada");

        if (avaliacao.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Você não tem permissão para excluir esta avaliação");

        await _avaliacaoRepository.RemoverAsync(avaliacao);
    }
}