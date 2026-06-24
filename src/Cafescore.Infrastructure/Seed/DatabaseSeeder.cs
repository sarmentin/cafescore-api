using Cafescore.Domain.Entities;
using Cafescore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cafescore.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Clinicas.AnyAsync())
            return;

        var clinicas = new List<Clinica>
        {
            new Clinica("Clínica São Lucas", "Av. Paulista, 1000", "São Paulo"),
            new Clinica("Centro Médico Vida", "Rua das Flores, 250", "Belo Horizonte"),
            new Clinica("Clínica Santa Maria", "Av. Atlântica, 500", "Rio de Janeiro"),
            new Clinica("Instituto de Saúde Bem Estar", "Rua XV de Novembro, 320", "Curitiba"),
            new Clinica("Clínica Esperança", "Av. Boa Viagem, 1500", "Recife")
        };

        await context.Clinicas.AddRangeAsync(clinicas);
        await context.SaveChangesAsync();
    }
}