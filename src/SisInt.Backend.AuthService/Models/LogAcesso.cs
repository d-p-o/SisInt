using System.ComponentModel.DataAnnotations;

namespace SisInt.Backend.AuthService.Models
{
    public class LogAcesso
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(36)]
        public required string UsuarioId { get; set; }

        public required Usuario Usuario { get; set; }

        public DateTime DataAcesso { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public required string IPOrigem { get; set; }

        [MaxLength(250)]
        public required string Detalhes { get; set; }
    }
}