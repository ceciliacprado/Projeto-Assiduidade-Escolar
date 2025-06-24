using API.Data;
using API.Models;

namespace API.Services;

public class DataInitializationService : IDataInitializationService
{
    private readonly AppDataContext _context;

    public DataInitializationService(AppDataContext context)
    {
        _context = context;
    }

    public async Task InitializeDefaultDataAsync()
    {
        // Verificar se já existem professores
        if (!_context.Professores.Any())
        {
            var professorPadrao = new Professor
            {
                Nome = "Administrador",
                Email = "admin@escola.com",
                Senha = "123456",
                Especialidade = "Administração"
            };

            _context.Professores.Add(professorPadrao);
            await _context.SaveChangesAsync();
        }

        // Verificar se já existem turmas
        if (!_context.Turmas.Any())
        {
            var turmaPadrao = new Turma
            {
                Nome = "Turma A",
                Ano = "2024",
                Serie = "1º"
            };

            _context.Turmas.Add(turmaPadrao);
            await _context.SaveChangesAsync();
        }

        // Verificar se já existem disciplinas
        if (!_context.Disciplinas.Any())
        {
            var turma = _context.Turmas.First();
            
            var disciplinasPadrao = new List<Disciplina>
            {
                new Disciplina
                {
                    Nome = "Matemática",
                    Codigo = "MAT001",
                    CargaHoraria = 80,
                    TurmaId = turma.Id
                },
                new Disciplina
                {
                    Nome = "Português",
                    Codigo = "PORT001",
                    CargaHoraria = 80,
                    TurmaId = turma.Id
                },
                new Disciplina
                {
                    Nome = "História",
                    Codigo = "HIST001",
                    CargaHoraria = 60,
                    TurmaId = turma.Id
                }
            };

            _context.Disciplinas.AddRange(disciplinasPadrao);
            await _context.SaveChangesAsync();
        }
    }
} 