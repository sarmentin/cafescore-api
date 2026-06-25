namespace Cafescore.Application.DTOs.Avaliacao;

public class CriarAvaliacaoDto
{
    public Guid ClinicaId { get; set; }
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
}