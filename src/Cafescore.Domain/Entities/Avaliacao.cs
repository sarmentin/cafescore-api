namespace Cafescore.Domain.Entities;

public class Avaliacao
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid ClinicaId { get; private set; }
    public int Nota { get; private set; }
    public string Comentario { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Clinica Clinica { get; private set; } = null!;

    protected Avaliacao() { }

    public Avaliacao(Guid usuarioId, Guid clinicaId, int nota, string comentario)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        ClinicaId = clinicaId;
        Nota = nota;
        Comentario = comentario;
        DataCriacao = DateTime.UtcNow;
    }

    public void Atualizar(int nota, string comentario)
    {
        Nota = nota;
        Comentario = comentario;
    }
}