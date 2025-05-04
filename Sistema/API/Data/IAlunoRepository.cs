using System;
using API.Models;

namespace API.Data;

public interface IAlunoRepository
{
    void Cadastrar(Aluno aluno);
    List<Aluno> Listar();
}
