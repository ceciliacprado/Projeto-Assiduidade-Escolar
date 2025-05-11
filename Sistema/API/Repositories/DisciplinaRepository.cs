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

    public Disciplina? BuscarPorId(int id)
    {
        return _context.Disciplinas.Find(id);
    }

    public void Atualizar(Disciplina disciplina)
    {
        _context.Disciplinas.Update(disciplina);
        _context.SaveChanges();
    }

    public void Deletar(int id)
    {
        var disciplina = _context.Disciplinas.Find(id);
        if (disciplina != null)
        {
            _context.Disciplinas.Remove(disciplina);
            _context.SaveChanges();
        }
    }
}
