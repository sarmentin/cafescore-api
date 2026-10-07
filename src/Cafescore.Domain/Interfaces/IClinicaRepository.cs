using Cafescore.Domain.Entities;

namespace Cafescore.Domain.Interfaces;

public interface IClinicaRepository
{
    Task<IEnumerable<Clinica>> ObterTodasAsync();
    Task<Clinica?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Clinica>> ObterPorOsmIdsAsync(IEnumerable<string> osmIds);
    Task AdicionarVariasAsync(IEnumerable<Clinica> clinicas);
    Task<IEnumerable<Clinica>> ObterNaAreaAsync(
    double latitudeMinima, double latitudeMaxima,
    double longitudeMinima, double longitudeMaxima);
}