using System;
using API.Models;

namespace API.Data;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDataContext _context;
    public ProdutoRepository(AppDataContext context)
    {
        _context = context;
    }
    public void Cadastrar(Produto produto)
    {
        _context.Produtos.Add(produto);
        _context.SaveChanges();
    }

    public List<Produto> Listar()
    {
        return _context.Produtos.ToList();
    }
}
