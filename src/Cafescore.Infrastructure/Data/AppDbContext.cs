using Cafescore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cafescore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Clinica> Clinicas => Set<Clinica>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.SenhaHash).IsRequired();
        });

        modelBuilder.Entity<Clinica>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Endereco).IsRequired().HasMaxLength(250);
            entity.Property(c => c.Cidade).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Nota).IsRequired();
            entity.Property(a => a.Comentario).IsRequired().HasMaxLength(500);

            entity.HasOne(a => a.Usuario)
                  .WithMany(u => u.Avaliacoes)
                  .HasForeignKey(a => a.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Clinica)
                  .WithMany(c => c.Avaliacoes)
                  .HasForeignKey(a => a.ClinicaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}