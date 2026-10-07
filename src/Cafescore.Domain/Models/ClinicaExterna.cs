namespace Cafescore.Domain.Models;

/// <summary>
/// Clínica como ela chega de uma fonte externa. Ainda não é uma entidade
/// do nosso sistema — é só o pacote de dados que atravessou a fronteira.
/// </summary>
public record ClinicaExterna(
    string OsmId,
    string Nome,
    string Endereco,
    string Cidade,
    double Latitude,
    double Longitude);