using API.Models;

namespace API.Services;

public interface IAlunoService
{
    Task<Aluno> CadastrarAlunoAsync(Aluno aluno);
    Task<List<Aluno>> ListarAlunosAsync();
    Task<Aluno?> BuscarAlunoPorIdAsync(int id);
    Task<Aluno> AtualizarAlunoAsync(int id, Aluno aluno);
    Task<bool> ExcluirAlunoAsync(int id);
    Task<List<Aluno>> ListarAlunosPorTurmaAsync(int turmaId);
} 