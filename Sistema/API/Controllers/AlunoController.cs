using API.Data;
using API.Models;
using API.Services;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunoController(IAlunoService alunoService)
    {
        _alunoService = alunoService;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarAlunoDTO dto)
    {
        try
        {
            var aluno = new Aluno
            {
                Nome = dto.Nome,
                TurmaId = dto.TurmaId
            };

            var alunoCadastrado = await _alunoService.CadastrarAlunoAsync(aluno);
            return Created("", alunoCadastrado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("listar")]
    [Authorize]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var alunos = await _alunoService.ListarAlunosAsync();
            return Ok(alunos);
        }
        catch
        {
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            var aluno = await _alunoService.BuscarAlunoPorIdAsync(id);
            if (aluno == null)
            {
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }
            return Ok(aluno);
        }
        catch
        {
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAlunoDTO dto)
    {
        try
        {
            var aluno = new Aluno
            {
                Nome = dto.Nome,
                TurmaId = dto.TurmaId
            };

            var alunoAtualizado = await _alunoService.AtualizarAlunoAsync(id, aluno);
            return Ok(alunoAtualizado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            var sucesso = await _alunoService.ExcluirAlunoAsync(id);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }
            return Ok(new { mensagem = "Aluno excluído com sucesso" });
        }
        catch
        {
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("turma/{turmaId}")]
    [Authorize]
    public async Task<IActionResult> ListarPorTurma(int turmaId)
    {
        try
        {
            var alunos = await _alunoService.ListarAlunosPorTurmaAsync(turmaId);
            return Ok(alunos);
        }
        catch
        {
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }
}