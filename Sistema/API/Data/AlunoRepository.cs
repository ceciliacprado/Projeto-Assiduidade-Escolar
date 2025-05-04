using System;
using API.Models;

namespace API.Data;

public class AlunoRepository : IAlunoRepository
{
    private readonly AppDataContext _context;
    public AlunoRepository(AppDataContext context)
    {
        _context = context;
    }
    public void Cadastrar(Aluno aluno)
    {
        _context.Alunos.Add(aluno);
        _context.SaveChanges();
    }

    public List<Aluno> Listar()
    {
        return _context.Alunos.ToList();
    }
}
