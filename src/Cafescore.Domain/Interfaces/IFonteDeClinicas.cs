using Cafescore.Domain.Models;

namespace Cafescore.Domain.Interfaces;

public interface IFonteDeClinicas
{
    Task<IReadOnlyList<ClinicaExterna>> BuscarProximasAsync(
        double latitude,
        double longitude,
        int raioMetros,
        CancellationToken cancellationToken = default);
}