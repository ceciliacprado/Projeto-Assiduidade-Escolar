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

        // Verificar se a turma tem disciplinas
        if (!turma.Disciplinas.Any())
        {
            throw new InvalidOperationException($"A turma '{turma.Nome}' não possui disciplinas cadastradas.");
        }

        // Cadastrar o aluno
        _context.Alunos.Add(aluno);
        await _context.SaveChangesAsync();

        // Vincular automaticamente o aluno a todas as disciplinas da turma
        foreach (var disciplina in turma.Disciplinas)
        {
            aluno.Disciplinas.Add(disciplina);
        }

        await _context.SaveChangesAsync();

        // Retornar o aluno com os relacionamentos carregados
        return await _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplinas)
            .FirstAsync(a => a.Id == aluno.Id);
    }

    public async Task<List<Aluno>> ListarAlunosAsync()
    {
        return await _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplinas)
            .ToListAsync();
    }

    public async Task<Aluno?> BuscarAlunoPorIdAsync(int id)
    {
        return await _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplinas)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Aluno> AtualizarAlunoAsync(int id, Aluno aluno)
    {
        var alunoExistente = await _context.Alunos
            .Include(a => a.Disciplinas)
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

            // Limpar disciplinas antigas
            alunoExistente.Disciplinas.Clear();

            // Adicionar novas disciplinas da turma
            foreach (var disciplina in novaTurma.Disciplinas)
            {
                alunoExistente.Disciplinas.Add(disciplina);
            }
        }

        // Atualizar dados do aluno
        alunoExistente.Nome = aluno.Nome;
        alunoExistente.TurmaId = aluno.TurmaId;

        await _context.SaveChangesAsync();

        return await _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplinas)
            .FirstAsync(a => a.Id == id);
    }

    public async Task<bool> ExcluirAlunoAsync(int id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno == null)
        {
            return false;
        }

        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Aluno>> ListarAlunosPorTurmaAsync(int turmaId)
    {
        return await _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplinas)
            .Where(a => a.TurmaId == turmaId)
            .ToListAsync();
    }
} 