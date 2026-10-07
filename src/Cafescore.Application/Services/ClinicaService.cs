using Cafescore.Application.DTOs.Clinica;
using Cafescore.Domain.Exceptions;
using Cafescore.Domain.Interfaces;
using ClinicaEntidade = Cafescore.Domain.Entities.Clinica;

namespace Cafescore.Application.Services;

public class ClinicaService
{
    /// <summary>
    /// A partir de quantas clínicas locais consideramos a região já conhecida
    /// e dispensamos a consulta externa.
    /// </summary>
    private const int MinimoParaUsarCache = 3;

    /// <summary>Metros por grau de latitude. Constante em qualquer lugar do globo.</summary>
    private const double MetrosPorGrauDeLatitude = 111_320d;

    private readonly IClinicaRepository _clinicaRepository;
    private readonly IFonteDeClinicas _fonteDeClinicas;

    public ClinicaService(IClinicaRepository clinicaRepository,
                          IFonteDeClinicas fonteDeClinicas)
    {
        _clinicaRepository = clinicaRepository;
        _fonteDeClinicas = fonteDeClinicas;
    }

    public async Task<IEnumerable<ClinicaDto>> ObterTodasAsync()
    {
        var clinicas = await _clinicaRepository.ObterTodasAsync();
        return clinicas.Select(Mapear);
    }

    public async Task<ClinicaDto> ObterPorIdAsync(Guid id)
    {
        var clinica = await _clinicaRepository.ObterPorIdAsync(id)
            ?? throw new RegraDeNegocioException("Clínica não encontrada");

        return Mapear(clinica);
    }

    /// <summary>
    /// Devolve as clínicas de uma região. Responde do banco quando já
    /// conhecemos o lugar; só consulta a fonte externa quando não conhecemos.
    /// </summary>
    public async Task<IEnumerable<ClinicaDto>> BuscarProximasAsync(
        double latitude,
        double longitude,
        int raioMetros,
        CancellationToken cancellationToken = default)
    {
        if (latitude is < -90 or > 90)
            throw new RegraDeNegocioException("Latitude inválida.");

        if (longitude is < -180 or > 180)
            throw new RegraDeNegocioException("Longitude inválida.");

        if (raioMetros is < 100 or > 10000)
            throw new RegraDeNegocioException("O raio deve estar entre 100 e 10000 metros.");

        var (latMin, latMax, lngMin, lngMax) = CalcularArea(latitude, longitude, raioMetros);

        // 1. Olha em casa primeiro.
        var locais = (await _clinicaRepository
            .ObterNaAreaAsync(latMin, latMax, lngMin, lngMax)).ToList();

        if (locais.Count >= MinimoParaUsarCache)
            return locais.Select(Mapear);

        // 2. Região desconhecida: consulta a fonte externa.
        var encontradas = await _fonteDeClinicas
            .BuscarProximasAsync(latitude, longitude, raioMetros, cancellationToken);

        // A consulta externa tem três ramos; o mesmo local pode casar com mais de um.
        var unicas = encontradas
            .GroupBy(c => c.OsmId)
            .Select(g => g.First())
            .ToList();

        if (unicas.Count > 0)
        {
            var conhecidos = (await _clinicaRepository
                    .ObterPorOsmIdsAsync(unicas.Select(c => c.OsmId)))
                .Select(c => c.OsmId!)
                .ToHashSet();

            var novas = unicas
                .Where(c => !conhecidos.Contains(c.OsmId))
                .Select(c => new ClinicaEntidade(
                    c.Nome, c.Endereco, c.Cidade, c.Latitude, c.Longitude, c.OsmId))
                .ToList();

            if (novas.Count > 0)
                await _clinicaRepository.AdicionarVariasAsync(novas);
        }

        // 3. Relê a área. Mais simples e mais seguro que juntar listas na mão:
        //    o banco é a única fonte de verdade sobre o que existe na região.
        var atualizadas = await _clinicaRepository
            .ObterNaAreaAsync(latMin, latMax, lngMin, lngMax);

        return atualizadas.Select(Mapear);
    }

    /// <summary>
    /// Converte um raio em metros numa caixa de latitudes e longitudes.
    /// Um grau de latitude vale sempre ~111 km; um grau de longitude encurta
    /// conforme você se afasta do equador, e é daí que vem o cosseno.
    /// </summary>
    private static (double LatMin, double LatMax, double LngMin, double LngMax)
        CalcularArea(double latitude, double longitude, int raioMetros)
    {
        var grausLatitude = raioMetros / MetrosPorGrauDeLatitude;

        // Perto dos polos o cosseno tende a zero; o piso evita divisão por algo
        // minúsculo, que geraria uma caixa do tamanho do planeta.
        var fator = Math.Max(Math.Cos(latitude * Math.PI / 180), 0.01);
        var grausLongitude = raioMetros / (MetrosPorGrauDeLatitude * fator);

        return (latitude - grausLatitude, latitude + grausLatitude,
                longitude - grausLongitude, longitude + grausLongitude);
    }

    private static ClinicaDto Mapear(ClinicaEntidade c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Endereco = c.Endereco,
        Cidade = c.Cidade,
        Latitude = c.Latitude,
        Longitude = c.Longitude,
        NotaMedia = c.Avaliacoes.Any()
            ? Math.Round(c.Avaliacoes.Average(a => a.Nota), 1)
            : 0,
        TotalAvaliacoes = c.Avaliacoes.Count
    };
}