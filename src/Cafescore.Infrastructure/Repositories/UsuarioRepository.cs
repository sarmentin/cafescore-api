using Cafescore.Domain.Entities;
using Cafescore.Domain.Interfaces;
using Cafescore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cafescore.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
        => await _context.Usuarios.FindAsync(id);

    public async Task<Usuario?> ObterPorEmailAsync(string email)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> EmailExisteAsync(string email)
        => await _context.Usuarios.AnyAsync(u => u.Email == email);

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }
}