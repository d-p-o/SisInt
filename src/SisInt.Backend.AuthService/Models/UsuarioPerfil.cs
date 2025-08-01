namespace SisInt.Backend.AuthService.Models
{
    public class UsuarioPerfil
    {
        public required string UsuarioId { get; set; }
        public required Usuario Usuario { get; set; }

        public int PerfilId { get; set; }
        public required Perfil Perfil { get; set; }
    }
}