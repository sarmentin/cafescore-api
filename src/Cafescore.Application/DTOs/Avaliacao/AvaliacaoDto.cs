namespace Cafescore.Application.DTOs.Avaliacao;

public class AvaliacaoDto
{
    public Guid Id { get; set; }
    public Guid ClinicaId { get; set; }
    public string NomeClinica { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}