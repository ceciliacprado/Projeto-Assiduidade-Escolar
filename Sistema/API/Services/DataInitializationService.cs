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
            var professores = new List<Professor>
            {
                new Professor
                {
                    Nome = "Administrador",
                    Email = "admin@escola.com",
                    Senha = "123456",
                    Especialidade = "Administração"
                },
                new Professor
                {
                    Nome = "Maria Silva",
                    Email = "maria.silva@escola.com",
                    Senha = "123456",
                    Especialidade = "Matemática"
                },
                new Professor
                {
                    Nome = "João Santos",
                    Email = "joao.santos@escola.com",
                    Senha = "123456",
                    Especialidade = "Português"
                }
            };

            _context.Professores.AddRange(professores);
            await _context.SaveChangesAsync();
        }

        // Verificar se já existem turmas
        if (!_context.Turmas.Any())
        {
            var turmas = new List<Turma>
            {
                new Turma
                {
                    Nome = "Turma A",
                    Ano = "2024",
                    Serie = "1º Ano"
                },
                new Turma
                {
                    Nome = "Turma B",
                    Ano = "2024",
                    Serie = "2º Ano"
                },
                new Turma
                {
                    Nome = "Turma C",
                    Ano = "2024",
                    Serie = "3º Ano"
                }
            };

            _context.Turmas.AddRange(turmas);
            await _context.SaveChangesAsync();
        }

        // Verificar se já existem disciplinas
        if (!_context.Disciplinas.Any())
        {
            var turmas = _context.Turmas.ToList();
            
            var disciplinas = new List<Disciplina>();
            
            foreach (var turma in turmas)
            {
                disciplinas.AddRange(new List<Disciplina>
                {
                    new Disciplina
                    {
                        Nome = "Matemática",
                        Codigo = $"MAT{turma.Id:D3}",
                        CargaHoraria = 80,
                        TurmaId = turma.Id
                    },
                    new Disciplina
                    {
                        Nome = "Português",
                        Codigo = $"PORT{turma.Id:D3}",
                        CargaHoraria = 80,
                        TurmaId = turma.Id
                    },
                    new Disciplina
                    {
                        Nome = "História",
                        Codigo = $"HIST{turma.Id:D3}",
                        CargaHoraria = 60,
                        TurmaId = turma.Id
                    }
                });
            }

            _context.Disciplinas.AddRange(disciplinas);
            await _context.SaveChangesAsync();
        }

        // Verificar se já existem alunos
        if (!_context.Alunos.Any())
        {
            var turmas = _context.Turmas.ToList();
            
            var alunos = new List<Aluno>();
            
            foreach (var turma in turmas)
            {
                alunos.AddRange(new List<Aluno>
                {
                    new Aluno
                    {
                        Nome = $"Ana Silva - {turma.Nome}",
                        TurmaId = turma.Id
                    },
                    new Aluno
                    {
                        Nome = $"Carlos Oliveira - {turma.Nome}",
                        TurmaId = turma.Id
                    },
                    new Aluno
                    {
                        Nome = $"Beatriz Costa - {turma.Nome}",
                        TurmaId = turma.Id
                    }
                });
            }

            _context.Alunos.AddRange(alunos);
            await _context.SaveChangesAsync();
        }

        // Verificar se já existem frequências
        if (!_context.Frequencias.Any())
        {
            var alunos = _context.Alunos.ToList();
            var disciplinas = _context.Disciplinas.ToList();
            
            var frequencias = new List<Frequencia>();
            var dataAtual = DateTime.Now;
            
            // Criar frequências para os últimos 30 dias
            for (int i = 0; i < 30; i++)
            {
                var data = dataAtual.AddDays(-i);
                
                // Pular fins de semana
                if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                    continue;
                
                foreach (var aluno in alunos)
                {
                    foreach (var disciplina in disciplinas.Where(d => d.TurmaId == aluno.TurmaId))
                    {
                        // Simular presença (80% de chance de estar presente)
                        var random = new Random();
                        var presente = random.Next(1, 101) <= 80;
                        
                        frequencias.Add(new Frequencia
                        {
                            AlunoId = aluno.Id,
                            DisciplinaId = disciplina.Id,
                            Data = data,
                            Presente = presente
                        });
                    }
                }
            }

            _context.Frequencias.AddRange(frequencias);
            await _context.SaveChangesAsync();
        }
    }
} 