using Cafescore.Domain.Entities;

namespace Cafescore.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
    Task AdicionarAsync(Usuario usuario);
}