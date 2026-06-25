namespace Cafescore.Application.DTOs.Avaliacao;

public class AtualizarAvaliacaoDto
{
    public int Nota { get; set; }
    public string Comentario { get; set; } = string.Empty;
}