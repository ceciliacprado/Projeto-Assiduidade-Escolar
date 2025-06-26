using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AlunoService : IAlunoService
{
    private readonly AppDataContext _context;

    public AlunoService(AppDataContext context)
    {
        _context = context;
    }

    public async Task<Aluno> CadastrarAlunoAsync(Aluno aluno)
    {
        // Verificar se a turma existe
        var turma = await _context.Turmas
            .Include(t => t.Disciplinas)
            .FirstOrDefaultAsync(t => t.Id == aluno.TurmaId);

        if (turma == null)
        {
            throw new InvalidOperationException($"Turma com ID {aluno.TurmaId} não encontrada.");
        }

        // Cadastrar o aluno
        _context.Alunos.Add(aluno);
        await _context.SaveChangesAsync();

        // Vincular automaticamente o aluno a todas as disciplinas da turma
        // através da criação de registros de frequência para a data atual
        var dataAtual = DateTime.Now.Date;
        var frequencias = new List<Frequencia>();
        
        foreach (var disciplina in turma.Disciplinas)
        {
            // Verificar se já existe registro de frequência para este aluno nesta disciplina nesta data
            var frequenciaExistente = await _context.Frequencias
                .FirstOrDefaultAsync(f => f.AlunoId == aluno.Id && 
                                        f.DisciplinaId == disciplina.Id && 
                                        f.Data.Date == dataAtual);
            
            if (frequenciaExistente == null)
            {
                frequencias.Add(new Frequencia
                {
                    AlunoId = aluno.Id,
                    DisciplinaId = disciplina.Id,
                    Data = dataAtual,
                    Presente = false // Inicialmente como ausente, será atualizado quando o professor fizer a chamada
                });
            }
        }

        if (frequencias.Any())
        {
            _context.Frequencias.AddRange(frequencias);
            await _context.SaveChangesAsync();
        }

        // Retornar o aluno com os relacionamentos carregados
        return await _context.Alunos
            .Include(a => a.Turma)
            .FirstAsync(a => a.Id == aluno.Id);
    }

    public async Task<List<Aluno>> ListarAlunosAsync()
    {
        return await _context.Alunos
            .Include(a => a.Turma)
            .ToListAsync();
    }

    public async Task<Aluno?> BuscarAlunoPorIdAsync(int id)
    {
        return await _context.Alunos
            .Include(a => a.Turma)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Aluno> AtualizarAlunoAsync(int id, Aluno aluno)
    {
        var alunoExistente = await _context.Alunos
            .FirstOrDefaultAsync(a => a.Id == id);
            
        if (alunoExistente == null)
        {
            throw new InvalidOperationException($"Aluno com ID {id} não encontrado.");
        }

        // Verificar se a nova turma existe
        if (aluno.TurmaId != alunoExistente.TurmaId)
        {
            var novaTurma = await _context.Turmas
                .Include(t => t.Disciplinas)
                .FirstOrDefaultAsync(t => t.Id == aluno.TurmaId);

            if (novaTurma == null)
            {
                throw new InvalidOperationException($"Turma com ID {aluno.TurmaId} não encontrada.");
            }

            // Remover frequências antigas (da turma anterior)
            var frequenciasAntigas = await _context.Frequencias
                .Where(f => f.AlunoId == id)
                .ToListAsync();
            
            _context.Frequencias.RemoveRange(frequenciasAntigas);

            // Criar novas frequências para a nova turma
            var dataAtual = DateTime.Now.Date;
            var novasFrequencias = new List<Frequencia>();
            
            foreach (var disciplina in novaTurma.Disciplinas)
            {
                novasFrequencias.Add(new Frequencia
                {
                    AlunoId = id,
                    DisciplinaId = disciplina.Id,
                    Data = dataAtual,
                    Presente = false
                });
            }

            if (novasFrequencias.Any())
            {
                _context.Frequencias.AddRange(novasFrequencias);
            }
        }

        // Atualizar dados do aluno
        alunoExistente.Nome = aluno.Nome;
        alunoExistente.TurmaId = aluno.TurmaId;

        await _context.SaveChangesAsync();

        return await _context.Alunos
            .Include(a => a.Turma)
            .FirstAsync(a => a.Id == id);
    }

    public async Task<bool> ExcluirAlunoAsync(int id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno == null)
        {
            return false;
        }

        // Remover frequências do aluno
        var frequencias = await _context.Frequencias
            .Where(f => f.AlunoId == id)
            .ToListAsync();
        
        _context.Frequencias.RemoveRange(frequencias);
        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Aluno>> ListarAlunosPorTurmaAsync(int turmaId)
    {
        try
        {
            Console.WriteLine($"ListarAlunosPorTurmaAsync chamado com turmaId: {turmaId}");
            
            var alunos = await _context.Alunos
                .Include(a => a.Turma)
                .Where(a => a.TurmaId == turmaId)
                .ToListAsync();
                
            Console.WriteLine($"Alunos encontrados: {alunos.Count}");
            return alunos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro em ListarAlunosPorTurmaAsync: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    // Novo método para obter disciplinas vinculadas ao aluno
    public async Task<List<Disciplina>> ObterDisciplinasDoAlunoAsync(int alunoId)
    {
        var disciplinas = await _context.Frequencias
            .Where(f => f.AlunoId == alunoId)
            .Include(f => f.Disciplina)
            .Select(f => f.Disciplina!)
            .Distinct()
            .ToListAsync();
        
        return disciplinas;
    }

    // Novo método para obter contagem de disciplinas do aluno
    public async Task<int> ObterContagemDisciplinasDoAlunoAsync(int alunoId)
    {
        return await _context.Frequencias
            .Where(f => f.AlunoId == alunoId)
            .Select(f => f.DisciplinaId)
            .Distinct()
            .CountAsync();
    }
} 