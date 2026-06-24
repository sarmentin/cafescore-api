using Cafescore.Domain.Entities;

namespace Cafescore.Domain.Interfaces;

public interface IAvaliacaoRepository
{
    Task<IEnumerable<Avaliacao>> ObterPorClinicaAsync(Guid clinicaId);
    Task<Avaliacao?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Avaliacao>> ObterPorUsuarioAsync(Guid usuarioId);
    Task AdicionarAsync(Avaliacao avaliacao);
    Task AtualizarAsync(Avaliacao avaliacao);
    Task RemoverAsync(Avaliacao avaliacao);
}