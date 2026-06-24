namespace Cafescore.Domain.Entities;

public class Clinica
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;

    public ICollection<Avaliacao> Avaliacoes { get; private set; } = null!;

    protected Clinica() { }

    public Clinica(string nome, string endereco, string cidade)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Endereco = endereco;
        Cidade = cidade;
        Avaliacoes = new List<Avaliacao>();
    }
}