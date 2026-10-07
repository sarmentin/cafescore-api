namespace Cafescore.Domain.Entities;

public class Clinica
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;

    // Coordenadas geográficas — base do geofencing da Fase 2
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    // Identificador no OpenStreetMap. Nulo para clínicas cadastradas manualmente.
    // É a chave que impede a mesma clínica de ser importada duas vezes.
    public string? OsmId { get; private set; }

    public ICollection<Avaliacao> Avaliacoes { get; private set; } = null!;

    protected Clinica() { }

    public Clinica(string nome, string endereco, string cidade,
                   double latitude, double longitude, string? osmId = null)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Endereco = endereco;
        Cidade = cidade;
        Latitude = latitude;
        Longitude = longitude;
        OsmId = osmId;
        Avaliacoes = new List<Avaliacao>();
    }

    /// <summary>
    /// Atualiza os dados vindos do OpenStreetMap numa reimportação.
    /// Nome e endereço podem mudar no OSM; a identidade da clínica (Id) não.
    /// </summary>
    public void AtualizarDadosOsm(string nome, string endereco, string cidade,
                                  double latitude, double longitude)
    {
        Nome = nome;
        Endereco = endereco;
        Cidade = cidade;
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Define as coordenadas de uma clínica cadastrada sem elas.
    /// </summary>
    public void DefinirCoordenadas(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}