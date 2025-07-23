using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SisInt.Backend.AuthService.Models
{
    public class Permissao
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Nome { get; set; }
        [MaxLength(250)]
        public required string Descricao { get; set; }

        public required ICollection<PermissaoPerfil> PermissaoPerfis { get; set; }
    }
}