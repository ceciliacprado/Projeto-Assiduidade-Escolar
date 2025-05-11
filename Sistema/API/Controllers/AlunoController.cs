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

    public AlunoController(IAlunoRepository alunoRepository, IConfiguration configuration)
    {
        _alunoRepository = alunoRepository;
        _configuration = configuration;
    }

    [HttpPost("cadastrar")]
    [Authorize(Roles = "admin")]
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