using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/produto")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    public ProdutoController(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "administrador")]
    public IActionResult Cadastrar([FromBody] Produto produto)
    {
        _produtoRepository.Cadastrar(produto);
        return Created("", produto);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {        
        return Ok(_produtoRepository.Listar());
    }
}