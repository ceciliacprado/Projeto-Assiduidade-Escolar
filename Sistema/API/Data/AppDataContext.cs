using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Frequencia> Frequencias { get; set; }
    public DbSet<Professor> Professores { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuração da entidade Aluno
        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Senha).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CriadoEm).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relacionamento com Disciplina (opcional)
            entity.HasOne(e => e.Disciplina)
                  .WithMany(d => d.Alunos)
                  .HasForeignKey(e => e.DisciplinaId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Configuração da entidade Disciplina
        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CriadoEm).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        // Configuração da entidade Frequencia
        modelBuilder.Entity<Frequencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.Presente).IsRequired();
            entity.Property(e => e.CriadoEm).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relacionamento com Aluno
            entity.HasOne(e => e.Aluno)
                  .WithMany(a => a.Frequencias)
                  .HasForeignKey(e => e.AlunoId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamento com Disciplina
            entity.HasOne(e => e.Disciplina)
                  .WithMany(d => d.Frequencias)
                  .HasForeignKey(e => e.DisciplinaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configuração da entidade Professor
        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Senha).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CriadoEm).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Índice único para email
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        // Índice único para email de Aluno
        modelBuilder.Entity<Aluno>()
            .HasIndex(e => e.Email)
            .IsUnique();
    }
}
