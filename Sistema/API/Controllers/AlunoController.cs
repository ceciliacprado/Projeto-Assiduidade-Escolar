using API.Data;
using API.Models;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly AppDataContext _context;
    private readonly IAlunoService _alunoService;

    public AlunoController(AppDataContext context, IAlunoService alunoService)
    {
        _context = context;
        _alunoService = alunoService;
    }

    [HttpGet("listar")]
    [Authorize]
    public async Task<IActionResult> Listar()
    {
        try
        {
            Console.WriteLine("Endpoint /api/aluno/listar foi chamado");
            
            var alunos = await _alunoService.ListarAlunosAsync();
            
            // Para cada aluno, obter a contagem de disciplinas
            var alunosComDisciplinas = new List<object>();
            foreach (var aluno in alunos)
            {
                var contagemDisciplinas = await _alunoService.ObterContagemDisciplinasDoAlunoAsync(aluno.Id);
                alunosComDisciplinas.Add(new
                {
                    id = aluno.Id,
                    nome = aluno.Nome,
                    criadoEm = aluno.CriadoEm,
                    turmaId = aluno.TurmaId,
                    turma = new TurmaDTO
                    {
                        Id = aluno.Turma.Id,
                        Nome = aluno.Turma.Nome,
                        Ano = aluno.Turma.Ano,
                        Serie = aluno.Turma.Serie,
                        CriadoEm = aluno.Turma.CriadoEm
                    },
                    contagemDisciplinas = contagemDisciplinas
                });
            }
                
            Console.WriteLine($"Total de alunos encontrados: {alunos.Count}");
            return Ok(alunosComDisciplinas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Listar: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id} foi chamado");
            
            var aluno = await _alunoService.BuscarAlunoPorIdAsync(id);
            if (aluno == null)
            {
                Console.WriteLine($"Aluno com ID {id} não encontrado");
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }

            var disciplinas = await _alunoService.ObterDisciplinasDoAlunoAsync(id);
            var contagemDisciplinas = await _alunoService.ObterContagemDisciplinasDoAlunoAsync(id);
            
            var alunoCompleto = new
            {
                id = aluno.Id,
                nome = aluno.Nome,
                criadoEm = aluno.CriadoEm,
                turmaId = aluno.TurmaId,
                turma = new TurmaDTO
                {
                    Id = aluno.Turma.Id,
                    Nome = aluno.Turma.Nome,
                    Ano = aluno.Turma.Ano,
                    Serie = aluno.Turma.Serie,
                    CriadoEm = aluno.Turma.CriadoEm
                },
                disciplinas = disciplinas.Select(d => new
                {
                    id = d.Id,
                    nome = d.Nome,
                    codigo = d.Codigo,
                    cargaHoraria = d.CargaHoraria
                }),
                contagemDisciplinas = contagemDisciplinas
            };
            
            Console.WriteLine($"Aluno encontrado: {aluno.Nome}");
            return Ok(alunoCompleto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint BuscarPorId: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpPost("cadastrar")]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarAlunoDTO dto)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/cadastrar foi chamado para: {dto.Nome}");
            
            // Validar dados obrigatórios
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new { mensagem = "Nome do aluno é obrigatório" });
            }
            
            // Cadastrar o aluno
            var aluno = new Aluno
            {
                Nome = dto.Nome,
                TurmaId = dto.TurmaId
            };
            
            var alunoCadastrado = await _alunoService.CadastrarAlunoAsync(aluno);
            
            // Obter disciplinas vinculadas
            var disciplinas = await _alunoService.ObterDisciplinasDoAlunoAsync(alunoCadastrado.Id);
            var contagemDisciplinas = await _alunoService.ObterContagemDisciplinasDoAlunoAsync(alunoCadastrado.Id);
            
            var resultado = new
            {
                id = alunoCadastrado.Id,
                nome = alunoCadastrado.Nome,
                criadoEm = alunoCadastrado.CriadoEm,
                turmaId = alunoCadastrado.TurmaId,
                turma = new TurmaDTO
                {
                    Id = alunoCadastrado.Turma.Id,
                    Nome = alunoCadastrado.Turma.Nome,
                    Ano = alunoCadastrado.Turma.Ano,
                    Serie = alunoCadastrado.Turma.Serie,
                    CriadoEm = alunoCadastrado.Turma.CriadoEm
                },
                contagemDisciplinas = contagemDisciplinas
            };
                
            Console.WriteLine($"Aluno cadastrado com sucesso: {alunoCadastrado.Nome}");
            return Created("", resultado);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Erro de validação: {ex.Message}");
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Cadastrar: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAlunoDTO dto)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id} (PUT) foi chamado");
            
            // Validar dados obrigatórios
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return BadRequest(new { mensagem = "Nome do aluno é obrigatório" });
            }
            
            // Atualizar o aluno
            var aluno = new Aluno
            {
                Nome = dto.Nome,
                TurmaId = dto.TurmaId
            };
            
            var alunoAtualizado = await _alunoService.AtualizarAlunoAsync(id, aluno);
            
            // Obter disciplinas vinculadas
            var disciplinas = await _alunoService.ObterDisciplinasDoAlunoAsync(id);
            var contagemDisciplinas = await _alunoService.ObterContagemDisciplinasDoAlunoAsync(id);
            
            var resultado = new
            {
                id = alunoAtualizado.Id,
                nome = alunoAtualizado.Nome,
                criadoEm = alunoAtualizado.CriadoEm,
                turmaId = alunoAtualizado.TurmaId,
                turma = new TurmaDTO
                {
                    Id = alunoAtualizado.Turma.Id,
                    Nome = alunoAtualizado.Turma.Nome,
                    Ano = alunoAtualizado.Turma.Ano,
                    Serie = alunoAtualizado.Turma.Serie,
                    CriadoEm = alunoAtualizado.Turma.CriadoEm
                },
                contagemDisciplinas = contagemDisciplinas
            };
                
            Console.WriteLine($"Aluno atualizado com sucesso: {alunoAtualizado.Nome}");
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Erro de validação: {ex.Message}");
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Atualizar: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id} (DELETE) foi chamado");
            
            var sucesso = await _alunoService.ExcluirAlunoAsync(id);
            if (!sucesso)
            {
                Console.WriteLine($"Aluno com ID {id} não encontrado");
                return NotFound(new { mensagem = "Aluno não encontrado" });
            }
            
            Console.WriteLine($"Aluno excluído com sucesso");
            return Ok(new { mensagem = "Aluno excluído com sucesso" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint Excluir: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("turma/{turmaId}")]
    [Authorize]
    public async Task<IActionResult> ListarPorTurma(int turmaId)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/turma/{turmaId} foi chamado");
            
            // Verificar se a turma existe
            var turma = await _context.Turmas.FindAsync(turmaId);
            if (turma == null)
            {
                Console.WriteLine($"Turma com ID {turmaId} não encontrada");
                return NotFound(new { mensagem = "Turma não encontrada" });
            }
            
            var alunos = await _alunoService.ListarAlunosPorTurmaAsync(turmaId);
            
            // Para cada aluno, obter a contagem de disciplinas
            var alunosComDisciplinas = new List<object>();
            foreach (var aluno in alunos)
            {
                var contagemDisciplinas = await _alunoService.ObterContagemDisciplinasDoAlunoAsync(aluno.Id);
                alunosComDisciplinas.Add(new
                {
                    id = aluno.Id,
                    nome = aluno.Nome,
                    criadoEm = aluno.CriadoEm,
                    turmaId = aluno.TurmaId,
                    turma = new TurmaDTO
                    {
                        Id = aluno.Turma.Id,
                        Nome = aluno.Turma.Nome,
                        Ano = aluno.Turma.Ano,
                        Serie = aluno.Turma.Serie,
                        CriadoEm = aluno.Turma.CriadoEm
                    },
                    contagemDisciplinas = contagemDisciplinas
                });
            }
                
            Console.WriteLine($"Alunos encontrados para turma {turma.Nome}: {alunos.Count}");
            return Ok(alunosComDisciplinas);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint ListarPorTurma: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("{id}/disciplinas")]
    [Authorize]
    public async Task<IActionResult> ObterDisciplinasDoAluno(int id)
    {
        try
        {
            Console.WriteLine($"Endpoint /api/aluno/{id}/disciplinas foi chamado");
            
            var disciplinas = await _alunoService.ObterDisciplinasDoAlunoAsync(id);
            var contagem = await _alunoService.ObterContagemDisciplinasDoAlunoAsync(id);
            
            var resultado = new
            {
                alunoId = id,
                disciplinas = disciplinas.Select(d => new
                {
                    id = d.Id,
                    nome = d.Nome,
                    codigo = d.Codigo,
                    cargaHoraria = d.CargaHoraria
                }),
                totalDisciplinas = contagem
            };
            
            Console.WriteLine($"Disciplinas encontradas para aluno {id}: {contagem}");
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no endpoint ObterDisciplinasDoAluno: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }

    [HttpGet("teste-banco")]
    public async Task<IActionResult> TesteBanco()
    {
        try
        {
            Console.WriteLine("Testando conexão com banco...");
            
            var totalAlunos = await _context.Alunos.CountAsync();
            var totalTurmas = await _context.Turmas.CountAsync();
            
            Console.WriteLine($"Total de alunos: {totalAlunos}");
            Console.WriteLine($"Total de turmas: {totalTurmas}");
            
            return Ok(new
            {
                totalAlunos,
                totalTurmas,
                mensagem = "Conexão com banco OK"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no teste do banco: {ex.Message}");
            return StatusCode(500, new { mensagem = $"Erro no banco: {ex.Message}" });
        }
    }

    [HttpPost("vincular-disciplinas-existente")]
    [Authorize]
    public async Task<IActionResult> VincularDisciplinasExistente()
    {
        try
        {
            Console.WriteLine("Iniciando vinculação de disciplinas para alunos existentes");
            
            var alunos = await _context.Alunos
                .Include(a => a.Turma)
                .ThenInclude(t => t.Disciplinas)
                .ToListAsync();
            
            var totalVinculados = 0;
            var dataAtual = DateTime.Now.Date;
            
            foreach (var aluno in alunos)
            {
                foreach (var disciplina in aluno.Turma.Disciplinas)
                {
                    // Verificar se já existe registro de frequência
                    var frequenciaExistente = await _context.Frequencias
                        .FirstOrDefaultAsync(f => f.AlunoId == aluno.Id && 
                                                f.DisciplinaId == disciplina.Id && 
                                                f.Data.Date == dataAtual);
                    
                    if (frequenciaExistente == null)
                    {
                        _context.Frequencias.Add(new Frequencia
                        {
                            AlunoId = aluno.Id,
                            DisciplinaId = disciplina.Id,
                            Data = dataAtual,
                            Presente = false
                        });
                        totalVinculados++;
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Vinculação concluída: {totalVinculados} registros criados");
            return Ok(new { 
                mensagem = "Vinculação concluída com sucesso", 
                totalVinculados = totalVinculados,
                totalAlunos = alunos.Count
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na vinculação: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro interno do servidor" });
        }
    }
}