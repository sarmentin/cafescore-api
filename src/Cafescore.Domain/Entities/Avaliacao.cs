namespace Cafescore.Domain.Entities;

public class Avaliacao
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid ClinicaId { get; private set; }
    public int Nota { get; private set; }
    public string Comentario { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; private set; }

    // URL pública da foto no Supabase Storage. Opcional.
    public string? UrlFoto { get; private set; }

    // Onde o usuário estava quando avaliou.
    // Nulo nas avaliações criadas antes do check-in existir.
    public double? LatitudeCheckIn { get; private set; }
    public double? LongitudeCheckIn { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Clinica Clinica { get; private set; } = null!;

    protected Avaliacao() { }

    public Avaliacao(Guid usuarioId, Guid clinicaId, int nota, string comentario,
                     double? latitudeCheckIn = null, double? longitudeCheckIn = null)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        ClinicaId = clinicaId;
        Nota = nota;
        Comentario = comentario;
        LatitudeCheckIn = latitudeCheckIn;
        LongitudeCheckIn = longitudeCheckIn;
        DataCriacao = DateTime.UtcNow;
    }

    public void Atualizar(int nota, string comentario)
    {
        Nota = nota;
        Comentario = comentario;
    }

    /// <summary>
    /// Associa a foto depois do upload para o Supabase Storage.
    /// Separado do construtor porque o upload acontece em outra requisição:
    /// primeiro a avaliação é criada, depois a imagem sobe.
    /// </summary>
    public void DefinirFoto(string urlFoto)
    {
        UrlFoto = urlFoto;
    }
}