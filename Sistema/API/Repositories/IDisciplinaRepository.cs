using System;
using API.Models;

namespace API.Repositories;

public interface IDisciplinaRepository
{
    void Cadastrar(Disciplina disciplina);
    List<Disciplina> Listar();
    Disciplina? BuscarPorId(int id);
    void Atualizar(Disciplina disciplina);
    void Deletar(int id);
}
