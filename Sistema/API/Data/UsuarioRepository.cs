using System;
using API.Models;

namespace API.Data;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDataContext _context;
    public UsuarioRepository(AppDataContext context)
    {
        _context = context;
    }

    public Usuario? BuscarUsuarioPorEmailSenha(string email, string senha)
    {
        Usuario? usuarioExistente = 
            _context.Usuarios.FirstOrDefault
            (x => x.Email == email && x.Senha == senha);
        return usuarioExistente;
    }

    public void Cadastrar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public List<Usuario> Listar()
    {
        return _context.Usuarios.ToList();
    }
}
