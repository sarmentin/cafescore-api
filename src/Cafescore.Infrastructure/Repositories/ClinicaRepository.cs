using Cafescore.Domain.Entities;
using Cafescore.Domain.Interfaces;
using Cafescore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cafescore.Infrastructure.Repositories;

public class ClinicaRepository : IClinicaRepository
{
    private readonly AppDbContext _context;

    public ClinicaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Clinica>> ObterTodasAsync()
        => await _context.Clinicas.ToListAsync();

    public async Task<Clinica?> ObterPorIdAsync(Guid id)
        => await _context.Clinicas
            .Include(c => c.Avaliacoes)
            .FirstOrDefaultAsync(c => c.Id == id);
}