using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Repositories;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IConfiguration _configuration;
    private readonly IDisciplinaRepository _disciplinaRepository;
    private readonly AppDataContext _context;

    public AlunoController(
        IAlunoRepository alunoRepository,
        IConfiguration configuration,
        IDisciplinaRepository disciplinaRepository,
        AppDataContext context)
    {
        _alunoRepository = alunoRepository;
        _configuration = configuration;
        _disciplinaRepository = disciplinaRepository;
        _context = context;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
    public IActionResult Cadastrar([FromBody] Aluno aluno)
    {
        if (string.IsNullOrWhiteSpace(aluno.Nome))
        {
            return BadRequest(new { mensagem = "Nome é obrigatório" });
        }

        _context.Alunos.Add(aluno);
        _context.SaveChanges();
        return Created("", aluno);
    }

    [HttpPost("vincular-disciplina")]
    [Authorize(Roles = "admin")]
    public IActionResult VincularDisciplina([FromBody] VincularDisciplinaRequest request)
    {
        var aluno = _context.Alunos.Find(request.AlunoId);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        var disciplina = _context.Disciplinas.Find(request.DisciplinaId);
        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada" });
        }

        aluno.DisciplinaId = request.DisciplinaId;
        _context.SaveChanges();

        return Ok(aluno);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {
        var alunos = _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplina)
            .ToList();
        return Ok(alunos);
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult BuscarPorId(int id)
    {
        var aluno = _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplina)
            .FirstOrDefault(a => a.Id == id);

        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }
        return Ok(aluno);
    }

    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] Aluno aluno)
    {
        var alunoExistente = _context.Alunos.Find(id);
        if (alunoExistente == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        alunoExistente.Nome = aluno.Nome;
        alunoExistente.TurmaId = aluno.TurmaId;
        alunoExistente.DisciplinaId = aluno.DisciplinaId;

        _context.SaveChanges();
        return Ok(alunoExistente);
    }

    [HttpDelete("{id}")]
    public IActionResult Excluir(int id)
    {
        var aluno = _context.Alunos.Find(id);
        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado" });
        }

        _context.Alunos.Remove(aluno);
        _context.SaveChanges();
        return Ok(new { mensagem = "Aluno excluído com sucesso" });
    }

    [HttpGet("turma/{turmaId}")]
    public IActionResult ListarPorTurma(int turmaId)
    {
        var alunos = _context.Alunos
            .Include(a => a.Turma)
            .Include(a => a.Disciplina)
            .Where(a => a.TurmaId == turmaId)
            .ToList();

        return Ok(alunos);
    }
}

public class VincularDisciplinaRequest
{
    public int AlunoId { get; set; }
    public int DisciplinaId { get; set; }
}