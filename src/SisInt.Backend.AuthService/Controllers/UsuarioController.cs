using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisInt.Backend.AuthService.Data;
using SisInt.Backend.AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace SisInt.Backend.AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Exige autenticação para todos os endpoints do controlador.
    public class UsuarioController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")] // Exige a role 'admin'.
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return Ok(await _context.Usuarios.ToListAsync());
        }

        /// <summary>
        /// Obtém um usuário por ID.
        /// </summary>
        /// <param name="id">O ID do usuário (Keycloak ID).</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")] // Permite acesso para 'admin' ou 'user'.
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
            // O 'sub' claim é o ID do usuário no Keycloak.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            // Permite acesso se o usuário for 'admin' ou se o ID solicitado for o seu próprio.
            if (User.IsInRole("admin") || (userIdClaim != null && userIdClaim == id))
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound("Usuário não encontrado.");
                }
                return Ok(usuario);
            }
            return Forbid("Você não tem permissão para acessar este recurso.");
        }
    }
}