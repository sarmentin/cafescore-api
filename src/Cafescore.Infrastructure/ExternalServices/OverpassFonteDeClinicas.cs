using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Cafescore.Domain.Interfaces;
using Cafescore.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Cafescore.Infrastructure.ExternalServices;

public class OverpassFonteDeClinicas : IFonteDeClinicas
{
    // O Overpass é mantido por voluntários e vive sobrecarregado.
    // Tentamos os espelhos em ordem; o primeiro que responder ganha.
    private static readonly string[] Espelhos =
    {
        "https://overpass.kumi.systems/api/interpreter",
        "https://overpass-api.de/api/interpreter",
        "https://overpass.private.coffee/api/interpreter"
    };

    private static readonly TimeSpan LimitePorEspelho = TimeSpan.FromSeconds(8);

    private readonly HttpClient _httpClient;
    private readonly ILogger<OverpassFonteDeClinicas> _logger;

    public OverpassFonteDeClinicas(HttpClient httpClient,
                                   ILogger<OverpassFonteDeClinicas> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ClinicaExterna>> BuscarProximasAsync(
        double latitude,
        double longitude,
        int raioMetros,
        CancellationToken cancellationToken = default)
    {
        var query = MontarQuery(latitude, longitude, raioMetros);

        foreach (var espelho in Espelhos)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var resultado = await TentarEspelhoAsync(espelho, query, cancellationToken);

            if (resultado is not null)
                return resultado;
        }

        _logger.LogWarning(
            "Nenhum espelho do Overpass respondeu para ({Lat}, {Lng})", latitude, longitude);

        return Array.Empty<ClinicaExterna>();
    }

    /// <summary>
    /// Devolve as clínicas se o espelho respondeu, ou null se falhou —
    /// para o chamador saber que deve tentar o próximo.
    /// </summary>
    private async Task<IReadOnlyList<ClinicaExterna>?> TentarEspelhoAsync(
        string url, string query, CancellationToken cancellationToken)
    {
        // Limite próprio por tentativa, sem perder o cancelamento do cliente.
        using var limite = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        limite.CancelAfter(LimitePorEspelho);

        try
        {
            using var corpo = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", query)
            });

            using var resposta = await _httpClient.PostAsync(url, corpo, limite.Token);

            if (!resposta.IsSuccessStatusCode)
            {
                _logger.LogWarning("{Url} respondeu {Status}", url, (int)resposta.StatusCode);
                return null;
            }

            var payload = await resposta.Content
                .ReadFromJsonAsync<OverpassResposta>(cancellationToken: limite.Token);

            if (payload is null)
                return null;

            var clinicas = payload.Elements
                .Select(Converter)
                .OfType<ClinicaExterna>()
                .ToList();

            _logger.LogInformation(
                "{Url} devolveu {Total} elementos, {Validos} aproveitados",
                url, payload.Elements.Count, clinicas.Count);

            return clinicas;
        }
        // Cancelamento do cliente: não é falha do espelho, deixa subir.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        // Timeout nosso ou erro de rede: devolve null para tentar o próximo.
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
        {
            _logger.LogWarning("{Url} falhou: {Motivo}", url, ex.Message);
            return null;
        }
    }

    private static string MontarQuery(double latitude, double longitude, int raioMetros)
    {
        // InvariantCulture é obrigatório: em pt-BR o separador decimal é vírgula,
        // e "-23,569" quebraria a query, que usa vírgula entre argumentos.
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lng = longitude.ToString(CultureInfo.InvariantCulture);

        // "nw" = nodes e ways. Relations ficam de fora: custam caro e, na
        // amostra que medimos, não trouxeram nenhuma clínica.
        return $"""
            [out:json][timeout:20];
            (
              nw["amenity"="clinic"](around:{raioMetros},{lat},{lng});
              nw["amenity"="doctors"](around:{raioMetros},{lat},{lng});
              nw["healthcare"="clinic"](around:{raioMetros},{lat},{lng});
            );
            out center;
            """;
    }

    private static ClinicaExterna? Converter(OverpassElemento elemento)
    {
        if (elemento.Tags is null)
            return null;

        // Sem nome não serve para exibir na tela.
        var nome = elemento.Tags.GetValueOrDefault("name");
        if (string.IsNullOrWhiteSpace(nome))
            return null;

        // Nó traz lat/lon no topo; way traz dentro de "center".
        var latitude = elemento.Lat ?? elemento.Center?.Lat;
        var longitude = elemento.Lon ?? elemento.Center?.Lon;
        if (latitude is null || longitude is null)
            return null;

        return new ClinicaExterna(
            OsmId: $"{elemento.Type}/{elemento.Id}",
            Nome: Truncar(nome, 150),
            Endereco: Truncar(MontarEndereco(elemento.Tags), 250),
            Cidade: Truncar(elemento.Tags.GetValueOrDefault("addr:city") ?? "Não informada", 100),
            Latitude: latitude.Value,
            Longitude: longitude.Value);
    }

    private static string MontarEndereco(Dictionary<string, string> tags)
    {
        var rua = tags.GetValueOrDefault("addr:street");
        var numero = tags.GetValueOrDefault("addr:housenumber");
        var bairro = tags.GetValueOrDefault("addr:suburb");

        if (!string.IsNullOrWhiteSpace(rua))
            return string.IsNullOrWhiteSpace(numero) ? rua : $"{rua}, {numero}";

        return !string.IsNullOrWhiteSpace(bairro) ? bairro : "Endereço não informado";
    }

    private static string Truncar(string valor, int tamanhoMaximo) =>
        valor.Length <= tamanhoMaximo ? valor : valor[..tamanhoMaximo];

    // ---- Formato da resposta do Overpass ----

    private sealed class OverpassResposta
    {
        [JsonPropertyName("elements")]
        public List<OverpassElemento> Elements { get; set; } = new();
    }

    private sealed class OverpassElemento
    {
        [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("lat")] public double? Lat { get; set; }
        [JsonPropertyName("lon")] public double? Lon { get; set; }
        [JsonPropertyName("center")] public OverpassCentro? Center { get; set; }
        [JsonPropertyName("tags")] public Dictionary<string, string>? Tags { get; set; }
    }

    private sealed class OverpassCentro
    {
        [JsonPropertyName("lat")] public double Lat { get; set; }
        [JsonPropertyName("lon")] public double Lon { get; set; }
    }
}