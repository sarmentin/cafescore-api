namespace Cafescore.Application.DTOs.Clinica;

public class ClinicaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public double NotaMedia { get; set; }
    public int TotalAvaliacoes { get; set; }
}