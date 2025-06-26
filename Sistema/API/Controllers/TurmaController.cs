using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/turma")]
public class TurmaController : ControllerBase
{
    private readonly AppDataContext _context;

    public TurmaController(AppDataContext context)
    {
        _context = context;
    }

    [HttpGet("teste")]
    public IActionResult Teste()    
    {
        return Ok(new { mensagem = "API de Turma está funcionando!", timestamp = DateTime.Now });
    }

    [HttpGet("teste-dados")]
    public IActionResult TesteDados()
    {
        var totalTurmas = _context.Turmas.Count();
        var totalAlunos = _context.Alunos.Count();
        var totalDisciplinas = _context.Disciplinas.Count();
        
        var turmaComAlunos = _context.Turmas
            .Include(t => t.Alunos)
            .Include(t => t.Disciplinas)
            .FirstOrDefault();
            
        return Ok(new
        {
            totalTurmas,
            totalAlunos,
            totalDisciplinas,
            turmaExemplo = turmaComAlunos != null ? new
            {
                id = turmaComAlunos.Id,
                nome = turmaComAlunos.Nome,
                alunosCount = turmaComAlunos.Alunos.Count,
                disciplinasCount = turmaComAlunos.Disciplinas.Count
            } : null
        });
    }

    [HttpGet("listar")]
    public IActionResult Listar()
    {
        var turmas = _context.Turmas.ToList();
        return Ok(turmas);
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] Turma turma)
    {
        if (string.IsNullOrWhiteSpace(turma.Nome))
        {
            return BadRequest(new { mensagem = "Nome da turma é obrigatório" });
        }

        _context.Turmas.Add(turma);
        _context.SaveChanges();
        return Created("", turma);
    }

    [HttpPost("recriar-dados-teste")]
    public IActionResult RecriarDadosTeste()
    {
        try
        {
            // Limpar dados existentes
            _context.Frequencias.RemoveRange(_context.Frequencias);
            _context.Alunos.RemoveRange(_context.Alunos);
            _context.Disciplinas.RemoveRange(_context.Disciplinas);
            _context.Turmas.RemoveRange(_context.Turmas);
            _context.SaveChanges();
            
            // Recriar turmas
            var turmas = new List<Turma>
            {
                new Turma { Nome = "Turma A", Ano = "2024", Serie = "1º Ano" },
                new Turma { Nome = "Turma B", Ano = "2024", Serie = "2º Ano" }
            };
            _context.Turmas.AddRange(turmas);
            _context.SaveChanges();
            
            // Recriar disciplinas
            var disciplinas = new List<Disciplina>();
            foreach (var turma in turmas)
            {
                disciplinas.AddRange(new List<Disciplina>
                {
                    new Disciplina { Nome = "Matemática", Codigo = $"MAT{turma.Id}", CargaHoraria = 80, TurmaId = turma.Id },
                    new Disciplina { Nome = "Português", Codigo = $"PORT{turma.Id}", CargaHoraria = 80, TurmaId = turma.Id }
                });
            }
            _context.Disciplinas.AddRange(disciplinas);
            _context.SaveChanges();
            
            // Recriar alunos
            var alunos = new List<Aluno>();
            foreach (var turma in turmas)
            {
                alunos.AddRange(new List<Aluno>
                {
                    new Aluno { Nome = $"Ana Silva - {turma.Nome}", TurmaId = turma.Id },
                    new Aluno { Nome = $"Carlos Oliveira - {turma.Nome}", TurmaId = turma.Id }
                });
            }
            _context.Alunos.AddRange(alunos);
            _context.SaveChanges();
            
            return Ok(new { mensagem = "Dados de teste recriados com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = $"Erro ao recriar dados: {ex.Message}" });
        }
    }

    [HttpGet("{id}")]
    public IActionResult BuscarPorId(int id)
    {
        var turma = _context.Turmas
            .Include(t => t.Alunos)
            .Include(t => t.Disciplinas)
            .FirstOrDefault(t => t.Id == id);
            
        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }
        
        return Ok(turma);
    }

    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] Turma turma)
    {
        var turmaExistente = _context.Turmas.Find(id);
        if (turmaExistente == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        turmaExistente.Nome = turma.Nome;
        turmaExistente.Ano = turma.Ano;
        turmaExistente.Serie = turma.Serie;

        _context.SaveChanges();
        return Ok(turmaExistente);
    }

    [HttpDelete("{id}")]
    public IActionResult Excluir(int id)
    {
        var turma = _context.Turmas.Find(id);
        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        _context.Turmas.Remove(turma);
        _context.SaveChanges();
        return Ok(new { mensagem = "Turma excluída com sucesso" });
    }

    [HttpGet("{id}/alunos")]
    public IActionResult ListarAlunos(int id)
    {
        var turma = _context.Turmas
            .Include(t => t.Alunos)
            .FirstOrDefault(t => t.Id == id);

        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        return Ok(turma.Alunos);
    }

    [HttpGet("{id}/disciplinas")]
    public IActionResult ListarDisciplinas(int id)
    {
        var turma = _context.Turmas
            .Include(t => t.Disciplinas)
            .FirstOrDefault(t => t.Id == id);

        if (turma == null)
        {
            return NotFound(new { mensagem = "Turma não encontrada" });
        }

        return Ok(turma.Disciplinas);
    }
} 