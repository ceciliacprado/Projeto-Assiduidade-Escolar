using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Repositories;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IConfiguration _configuration;
    private readonly IDisciplinaRepository _disciplinaRepository;

    public AlunoController(
        IAlunoRepository alunoRepository,
        IConfiguration configuration,
        IDisciplinaRepository disciplinaRepository)
    {
        _alunoRepository = alunoRepository;
        _configuration = configuration;
        _disciplinaRepository = disciplinaRepository;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
    public IActionResult Cadastrar([FromBody] Aluno aluno)
    {
        aluno.CriadoEm = DateTime.Now;
        _alunoRepository.Cadastrar(aluno);
        return Created("", aluno);
    }

    [HttpPost("vincular-disciplina")]
    [Authorize(Roles = "admin")]
    public IActionResult VincularDisciplina([FromBody] VincularDisciplinaRequest request)
    {
        var aluno = _alunoRepository.BuscarPorId(request.AlunoId);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        var disciplina = _disciplinaRepository.BuscarPorId(request.DisciplinaId);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        aluno.DisciplinaId = request.DisciplinaId;
        _alunoRepository.Atualizar(aluno);
        return Ok(aluno);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {
        return Ok(_alunoRepository.Listar());
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult BuscarPorId(int id)
    {
        return Ok(_alunoRepository.BuscarPorId(id));
    }
}

public class VincularDisciplinaRequest
{
    public int AlunoId { get; set; }
    public int DisciplinaId { get; set; }
}