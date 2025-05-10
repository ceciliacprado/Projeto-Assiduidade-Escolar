using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Frequencia> Frequencias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

}
