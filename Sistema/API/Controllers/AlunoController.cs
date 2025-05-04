using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoRepository _alunoRepository;
    public AlunoController(IAlunoRepository alunoRepository)
    {
        _alunoRepository = alunoRepository;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "administrador")]
    public IActionResult Cadastrar([FromBody] Aluno aluno)
    {
        _alunoRepository.Cadastrar(aluno);
        return Created("", aluno);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {        
        return Ok(_alunoRepository.Listar());
    }
}