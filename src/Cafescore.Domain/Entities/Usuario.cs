namespace Cafescore.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }

    public ICollection<Avaliacao> Avaliacoes { get; private set; } = null!;

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        DataCriacao = DateTime.UtcNow;
        Avaliacoes = new List<Avaliacao>();
    }
}