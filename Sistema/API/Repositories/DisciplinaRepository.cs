using System;
using API.Models;
using API.Data;

namespace API.Repositories;

public class DisciplinaRepository : IDisciplinaRepository
{
    private readonly AppDataContext _context;
    public DisciplinaRepository(AppDataContext context)
    {
        _context = context;
    }
    public void Cadastrar(Disciplina disciplina)
    {
        _context.Disciplinas.Add(disciplina);
        _context.SaveChanges();
    }

    public List<Disciplina> Listar()
    {
        return _context.Disciplinas.ToList();
    }
}
