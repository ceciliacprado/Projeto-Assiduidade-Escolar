using System;
using API.Models;

namespace API.Data;

public interface IProdutoRepository
{
    void Cadastrar(Produto produto);
    List<Produto> Listar();
}
