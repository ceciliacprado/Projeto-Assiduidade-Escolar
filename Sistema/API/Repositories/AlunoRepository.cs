using System;
using API.Models;
using API.Data;

namespace API.Repositories;

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

    public Aluno? BuscarPorId(int id)
    {
        return _context.Alunos.Find(id);
    }

    public void Atualizar(Aluno aluno)
    {
        _context.Alunos.Update(aluno);
        _context.SaveChanges();
    }
}
