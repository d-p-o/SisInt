using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SisInt.Backend.AuthService.Models
{
    /// <summary>
    /// Entidade que representa uma permissão de acesso a recursos.
    /// </summary>
    public class Permissao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Nome { get; set; }

        [MaxLength(250)]
        public required string Descricao { get; set; }

        // Propriedade de navegação para o relacionamento muitos-para-muitos.
        public required ICollection<PermissaoPerfil> PermissaoPerfis { get; set; }
    }
}