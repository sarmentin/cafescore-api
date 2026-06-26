using Cafescore.Application.DTOs.Auth;
using Cafescore.Application.Services;
using Cafescore.Domain.Entities;
using Cafescore.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Cafescore.Application.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["Jwt:Key"])
            .Returns("CafescoreApp@2026#SecretKey!MtSdH");
        _configurationMock.Setup(c => c["Jwt:Issuer"])
            .Returns("cafescore-api");
        _configurationMock.Setup(c => c["Jwt:Audience"])
            .Returns("cafescore-app");
        _configurationMock.Setup(c => c["Jwt:ExpiresInHours"])
            .Returns("8");

        _authService = new AuthService(_usuarioRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task Registrar_DeveRetornarToken_QuandoDadosValidos()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto
        {
            Nome = "Teste",
            Email = "teste@cafescore.com",
            Senha = "123456"
        };

        _usuarioRepositoryMock
            .Setup(r => r.EmailExisteAsync(dto.Email))
            .ReturnsAsync(false);

        _usuarioRepositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _authService.RegistrarAsync(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado.Token);
        Assert.Equal(dto.Email, resultado.Email);
        Assert.Equal(dto.Nome, resultado.Nome);
    }

    [Fact]
    public async Task Registrar_DeveLancarExcecao_QuandoEmailJaCadastrado()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto
        {
            Nome = "Teste",
            Email = "existente@cafescore.com",
            Senha = "123456"
        };

        _usuarioRepositoryMock
            .Setup(r => r.EmailExisteAsync(dto.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _authService.RegistrarAsync(dto));
    }

    [Fact]
    public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
    {
        // Arrange
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var usuario = new Usuario("Teste", "teste@cafescore.com", senhaHash);

        var dto = new LoginDto
        {
            Email = "teste@cafescore.com",
            Senha = "123456"
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorEmailAsync(dto.Email))
            .ReturnsAsync(usuario);

        // Act
        var resultado = await _authService.LoginAsync(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado.Token);
    }

    [Fact]
    public async Task Login_DeveLancarExcecao_QuandoSenhaInvalida()
    {
        // Arrange
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta");
        var usuario = new Usuario("Teste", "teste@cafescore.com", senhaHash);

        var dto = new LoginDto
        {
            Email = "teste@cafescore.com",
            Senha = "senhaErrada"
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorEmailAsync(dto.Email))
            .ReturnsAsync(usuario);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _authService.LoginAsync(dto));
    }

    [Fact]
    public async Task Login_DeveLancarExcecao_QuandoEmailNaoEncontrado()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "naoexiste@cafescore.com",
            Senha = "123456"
        };

        _usuarioRepositoryMock
            .Setup(r => r.ObterPorEmailAsync(dto.Email))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _authService.LoginAsync(dto));
    }
}