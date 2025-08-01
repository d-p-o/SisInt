using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SisInt.Backend.AuthService.Models
{
    /// <summary>
    /// Entidade que representa um perfil de usuário (e.g., 'admin', 'user').
    /// </summary>
    public class Perfil
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Nome { get; set; }

        [MaxLength(250)]
        public required string Descricao { get; set; }

        // Propriedades de navegação para os relacionamentos muitos-para-muitos.
        public required ICollection<PermissaoPerfil> PermissaoPerfis { get; set; }
        public required ICollection<UsuarioPerfil> UsuarioPerfis { get; set; }
    }
}