using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SisInt.Backend.AuthService.Models
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Username { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Email { get; set; }
        public bool EmailConfirmado { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public required ICollection<UsuarioPerfil> UsuarioPerfis { get; set; }
        public required ICollection<LogAcesso> LogAcessos { get; set; }
    }
}