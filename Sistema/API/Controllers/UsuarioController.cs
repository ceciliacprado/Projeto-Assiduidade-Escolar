using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data;
using API.Repositories;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/usuario")]
public class UsuarioController : ControllerBase
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly IConfiguration _configuration;

    public UsuarioController(
        IProfessorRepository professorRepository,
        IAlunoRepository alunoRepository,
        IConfiguration configuration)
    {
        _professorRepository = professorRepository;
        _alunoRepository = alunoRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Primeiro tenta fazer login como professor
        Professor? professor = _professorRepository.BuscarProfessorPorEmailSenha(request.Email, request.Senha);
        if (professor != null)
        {
            string token = GerarToken(professor.Email, "admin");
            return Ok(token);
        }

        // Se não for professor, tenta como aluno
        var alunos = _alunoRepository.Listar();
        Aluno? aluno = alunos.FirstOrDefault(a => a.Email == request.Email && a.Senha == request.Senha);
        if (aluno != null)
        {
            string token = GerarToken(aluno.Email, "aluno");
            return Ok(token);
        }

        return Unauthorized(new { mensagem = "Email ou senha inválidos!" });
    }

    private string GerarToken(string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        };

        var chave = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        var assinatura = new SigningCredentials(
            new SymmetricSecurityKey(chave),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddSeconds(3000000),
            signingCredentials: assinatura
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
} 