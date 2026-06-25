namespace Cafescore.Application.DTOs.Auth;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
}