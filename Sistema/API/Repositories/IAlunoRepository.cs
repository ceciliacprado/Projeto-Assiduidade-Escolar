using System;
using API.Models;

namespace API.Repositories;

public interface IAlunoRepository
{
    void Cadastrar(Aluno aluno);
    List<Aluno> Listar();
    Aluno? BuscarPorId(int id);
    void Atualizar(Aluno aluno);
}
