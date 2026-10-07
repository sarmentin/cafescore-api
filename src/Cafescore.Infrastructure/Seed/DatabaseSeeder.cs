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
            new("Clínica São Lucas",             "Av. Paulista, 1000",        "São Paulo",      -23.5690, -46.6459),
            new("Centro Médico Vida",            "Rua das Flores, 250",       "Belo Horizonte", -19.9208, -43.9378),
            new("Clínica Santa Maria",           "Av. Atlântica, 500",        "Rio de Janeiro", -22.9630, -43.1725),
            new("Instituto de Saúde Bem Estar",  "Rua XV de Novembro, 320",   "Curitiba",       -25.4297, -49.2680),
            new("Clínica Esperança",             "Av. Boa Viagem, 1500",      "Recife",          -8.1180, -34.8970)
        };

        await context.Clinicas.AddRangeAsync(clinicas);
        await context.SaveChangesAsync();
    }
}