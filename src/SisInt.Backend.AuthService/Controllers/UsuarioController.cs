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
    [Authorize] // Todos os endpoints aqui exigem autenticação por padrão
    public class UsuarioController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Obtém todos os usuários registrados localmente.
        /// </summary>
        /// <remarks>
        /// Requer a role 'admin'.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return Ok(await _context.Usuarios.ToListAsync());
        }

        /// <summary>
        /// Obtém um usuário específico por ID.
        /// </summary>
        /// <param name="id">ID do usuário (Guid).</param>
        /// <remarks>
        /// Requer a role 'admin' ou ser o próprio usuário.
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<Usuario>> GetUsuario(Guid id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("admin") || (userIdClaim != null && Guid.TryParse(userIdClaim, out var guid) && guid == id))
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return Ok(usuario);
            }
            return Forbid();
        }

        /// <summary>
        /// Registra um novo usuário no banco de dados local.
        /// </summary>
        /// <remarks>
        /// Este endpoint pode ser usado para sincronizar usuários do Keycloak para o banco de dados local,
        /// ou para registrar usuários que não são inicialmente gerenciados pelo Keycloak, mas exigem
        /// uma entrada no banco de dados do SisInt.
        /// Requer a role 'admin'.
        /// </remarks>
        [HttpPost]
        [AllowAnonymous] // Pode ser acessado sem autenticação inicial (para registro público, por exemplo)
        public async Task<ActionResult<Usuario>> RegisterUsuario([FromBody] UsuarioRegisterDto usuarioDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
            {
                return Conflict("Um usuário com este email já existe.");
            }
            var novoUsuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Username = usuarioDto.Username,
                Email = usuarioDto.Email,
                EmailConfirmado = false,
                DataCriacao = DateTime.UtcNow,
                UsuarioPerfis = [],
                LogAcessos = []
            };
            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();
            _context.LogAcessos.Add(new LogAcesso
            {
                UsuarioId = novoUsuario.Id,
                Usuario = novoUsuario,
                DataAcesso = DateTime.UtcNow,
                IPOrigem = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "N/A",
                Detalhes = $"Usuário {novoUsuario.Username} registrado localmente."
            });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsuario), new { id = novoUsuario.Id }, novoUsuario);
        }

        public class UsuarioRegisterDto
        {
            [Required]
            public required string Username { get; set; }
            [Required]
            [EmailAddress]
            public required string Email { get; set; }
            // public Guid? KeycloakId { get; set; } // Opcional
        }
    }
}
