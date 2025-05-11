using System;
using API.Models;
using API.Data;

namespace API.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly AppDataContext _context;
    public ProfessorRepository(AppDataContext context)
    {
        _context = context;
    }

    public Professor? BuscarProfessorPorEmailSenha(string email, string senha)
    {
        Professor? professorExistente =
            _context.Professores.FirstOrDefault
            (x => x.Email == email && x.Senha == senha);
        return professorExistente;
    }

    public void Cadastrar(Professor professor)
    {
        _context.Professores.Add(professor);
        _context.SaveChanges();
    }

    public List<Professor> Listar()
    {
        return _context.Professores.ToList();
    }
}
