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
        => await _context.Clinicas
            .Include(c => c.Avaliacoes)
            .ToListAsync();

    public async Task<Clinica?> ObterPorIdAsync(Guid id)
        => await _context.Clinicas
            .Include(c => c.Avaliacoes)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Clinica>> ObterPorOsmIdsAsync(IEnumerable<string> osmIds)
    {
        var ids = osmIds.ToList();

        if (ids.Count == 0)
            return new List<Clinica>();

        return await _context.Clinicas
            .Include(c => c.Avaliacoes)
            .Where(c => c.OsmId != null && ids.Contains(c.OsmId))
            .ToListAsync();
    }

    public async Task AdicionarVariasAsync(IEnumerable<Clinica> clinicas)
    {
        await _context.Clinicas.AddRangeAsync(clinicas);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Clinica>> ObterNaAreaAsync(
    double latitudeMinima, double latitudeMaxima,
    double longitudeMinima, double longitudeMaxima)
    => await _context.Clinicas
        .Include(c => c.Avaliacoes)
        .Where(c => c.Latitude >= latitudeMinima && c.Latitude <= latitudeMaxima
                 && c.Longitude >= longitudeMinima && c.Longitude <= longitudeMaxima)
        .ToListAsync();
}