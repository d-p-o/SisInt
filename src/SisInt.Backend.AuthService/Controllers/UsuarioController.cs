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
    [Authorize]
    public class UsuarioController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return Ok(await _context.Usuarios.ToListAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

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

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Usuario>> RegisterUsuario([FromBody] UsuarioRegisterDto usuarioDto)
        {
            if (string.IsNullOrWhiteSpace(usuarioDto.KeycloakId))
            {
                return BadRequest("O KeycloakId é obrigatório.");
            }

            if (await _context.Usuarios.AnyAsync(u => u.Id == usuarioDto.KeycloakId))
            {
                return Conflict("Um usuário com este KeycloakId já existe.");
            }

            var novoUsuario = new Usuario
            {
                Id = usuarioDto.KeycloakId,
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
                Detalhes = $"Usuário {novoUsuario.Username} registrado por administrador."
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

            [Required]
            public required string KeycloakId { get; set; }
        }
    }
}