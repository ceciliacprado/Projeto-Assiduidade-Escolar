using System;
using API.Models;

namespace API.Repositories;

public interface IDisciplinaRepository
{
    void Cadastrar(Disciplina disciplina);
    List<Disciplina> Listar();
}
