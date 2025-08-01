using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SisInt.Backend.AuthService.Models
{
    /// <summary>
    /// Entidade que representa um usuário, sincronizado com o Keycloak.
    /// O Id é o 'sub' claim do token JWT.
    /// </summary>
    public class Usuario
    {
        [Key]
        [MaxLength(36)]
        public required string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(256)]
        public required string Email { get; set; }

        public bool EmailConfirmado { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Propriedades de navegação para os relacionamentos.
        public required ICollection<UsuarioPerfil> UsuarioPerfis { get; set; }
        public required ICollection<LogAcesso> LogAcessos { get; set; }
    }
}