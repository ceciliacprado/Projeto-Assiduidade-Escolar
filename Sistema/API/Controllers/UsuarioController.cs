using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/aluno")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly IConfiguration _configuration;
    public AlunoController(IAlunoRepository alunoRepository,
        IConfiguration configuration)
    {
        _alunoRepository = alunoRepository;
        _configuration = configuration;
    }

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] Aluno aluno)
    {
        _alunoRepository.Cadastrar(aluno);
        return Created("", aluno);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Aluno aluno)
    {
        Aluno? alunoExistente = _alunoRepository
            .BuscarAlunoPorEmailSenha(aluno.Email, aluno.Senha);

        if (alunoExistente == null)
        {
            return Unauthorized(new { mensagem = "Aluno ou senha inválidos!" });
        }

        string token = GerarToken(alunoExistente);
        return Ok(token);
    }

    [HttpGet("listar")]
    public IActionResult Listar()
    {
        return Ok(_alunoRepository.Listar());
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public string GerarToken(Aluno aluno)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, aluno.Email),
            new Claim(ClaimTypes.Role, aluno.Permissao.ToString())
        };

        var chave = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
        
        var assinatura = new SigningCredentials(
            new SymmetricSecurityKey(chave),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddSeconds(30),
            signingCredentials: assinatura
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}