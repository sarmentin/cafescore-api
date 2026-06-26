using Cafescore.Application.DTOs.Avaliacao;
using Cafescore.Application.Services;
using Cafescore.Domain.Entities;
using Cafescore.Domain.Interfaces;
using Moq;

namespace Cafescore.Application.Tests;

public class AvaliacaoServiceTests
{
    private readonly Mock<IAvaliacaoRepository> _avaliacaoRepositoryMock;
    private readonly Mock<IClinicaRepository> _clinicaRepositoryMock;
    private readonly AvaliacaoService _avaliacaoService;

    public AvaliacaoServiceTests()
    {
        _avaliacaoRepositoryMock = new Mock<IAvaliacaoRepository>();
        _clinicaRepositoryMock = new Mock<IClinicaRepository>();

        _avaliacaoService = new AvaliacaoService(
            _avaliacaoRepositoryMock.Object,
            _clinicaRepositoryMock.Object);
    }

    [Fact]
    public async Task Criar_DeveRetornarAvaliacao_QuandoDadosValidos()
    {
        // Arrange
        var clinicaId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var clinica = new Clinica("Clínica Teste", "Rua Teste, 100", "São Paulo");

        var dto = new CriarAvaliacaoDto
        {
            ClinicaId = clinicaId,
            Nota = 5,
            Comentario = "Café excelente!"
        };

        _clinicaRepositoryMock
            .Setup(r => r.ObterPorIdAsync(clinicaId))
            .ReturnsAsync(clinica);

        _avaliacaoRepositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Avaliacao>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _avaliacaoService.CriarAsync(dto, usuarioId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(dto.Nota, resultado.Nota);
        Assert.Equal(dto.Comentario, resultado.Comentario);
    }

    [Fact]
    public async Task Criar_DeveLancarExcecao_QuandoClinicaNaoEncontrada()
    {
        // Arrange
        var dto = new CriarAvaliacaoDto
        {
            ClinicaId = Guid.NewGuid(),
            Nota = 4,
            Comentario = "Bom café"
        };

        _clinicaRepositoryMock
            .Setup(r => r.ObterPorIdAsync(dto.ClinicaId))
            .ReturnsAsync((Clinica?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _avaliacaoService.CriarAsync(dto, Guid.NewGuid()));
    }

    [Fact]
    public async Task Atualizar_DeveLancarUnauthorized_QuandoUsuarioNaoEDono()
    {
        // Arrange
        var donoId = Guid.NewGuid();
        var outroUsuarioId = Guid.NewGuid();
        var avaliacao = new Avaliacao(donoId, Guid.NewGuid(), 4, "Bom café");

        var dto = new AtualizarAvaliacaoDto { Nota = 3, Comentario = "Mudei de ideia" };

        _avaliacaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(avaliacao.Id))
            .ReturnsAsync(avaliacao);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _avaliacaoService.AtualizarAsync(avaliacao.Id, dto, outroUsuarioId));
    }

    [Fact]
    public async Task Remover_DeveLancarUnauthorized_QuandoUsuarioNaoEDono()
    {
        // Arrange
        var donoId = Guid.NewGuid();
        var outroUsuarioId = Guid.NewGuid();
        var avaliacao = new Avaliacao(donoId, Guid.NewGuid(), 4, "Bom café");

        _avaliacaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(avaliacao.Id))
            .ReturnsAsync(avaliacao);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _avaliacaoService.RemoverAsync(avaliacao.Id, outroUsuarioId));
    }

    [Fact]
    public async Task Remover_DeveLancarExcecao_QuandoAvaliacaoNaoEncontrada()
    {
        // Arrange
        _avaliacaoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Avaliacao?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _avaliacaoService.RemoverAsync(Guid.NewGuid(), Guid.NewGuid()));
    }
}