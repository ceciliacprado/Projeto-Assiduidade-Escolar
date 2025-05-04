using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions options) : 
        base(options) { }
    public DbSet<Aluno> Alunos { get; set; }
}
