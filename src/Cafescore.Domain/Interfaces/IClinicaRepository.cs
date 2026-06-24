using Cafescore.Domain.Entities;

namespace Cafescore.Domain.Interfaces;

public interface IClinicaRepository
{
    Task<IEnumerable<Clinica>> ObterTodasAsync();
    Task<Clinica?> ObterPorIdAsync(Guid id);
}