using System;
using API.Models;

namespace API.Repositories;

public interface IProfessorRepository
{
    void Cadastrar(Professor professor);
    List<Professor> Listar();
    Professor? BuscarProfessorPorEmailSenha(string email, string senha);
}
