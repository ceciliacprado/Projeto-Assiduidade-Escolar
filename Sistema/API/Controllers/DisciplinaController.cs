using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/disciplina")]
public class DisciplinaController : ControllerBase
{
    private readonly IDisciplinaRepository _disciplinaRepository;

    public DisciplinaController(IDisciplinaRepository disciplinaRepository)
    {
        _disciplinaRepository = disciplinaRepository;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
    public IActionResult Cadastrar([FromBody] Disciplina disciplina)
    {
        _disciplinaRepository.Cadastrar(disciplina);
        return Created("", disciplina);
    }

    [HttpGet("listar")]
    [Authorize]
    public IActionResult Listar()
    {
        return Ok(_disciplinaRepository.Listar());
    }
}
