using Cafescore.Domain.Entities;
using Cafescore.Domain.Interfaces;
using Cafescore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cafescore.Infrastructure.Repositories;

public class AvaliacaoRepository : IAvaliacaoRepository
{
    private readonly AppDbContext _context;

    public AvaliacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Avaliacao>> ObterPorClinicaAsync(Guid clinicaId)
        => await _context.Avaliacoes
            .Include(a => a.Usuario)
            .Where(a => a.ClinicaId == clinicaId)
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();

    public async Task<Avaliacao?> ObterPorIdAsync(Guid id)
        => await _context.Avaliacoes
            .Include(a => a.Usuario)
            .Include(a => a.Clinica)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Avaliacao>> ObterPorUsuarioAsync(Guid usuarioId)
        => await _context.Avaliacoes
            .Include(a => a.Clinica)
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();

    public async Task AdicionarAsync(Avaliacao avaliacao)
    {
        await _context.Avaliacoes.AddAsync(avaliacao);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Avaliacao avaliacao)
    {
        _context.Avaliacoes.Update(avaliacao);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Avaliacao avaliacao)
    {
        _context.Avaliacoes.Remove(avaliacao);
        await _context.SaveChangesAsync();
    }
}