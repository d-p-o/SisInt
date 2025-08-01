namespace SisInt.Backend.AuthService.Models
{
    /// <summary>
    /// Entidade de junção para o relacionamento muitos-para-muitos entre Usuario e Perfil.
    /// </summary>
    public class UsuarioPerfil
    {
        public required string UsuarioId { get; set; }
        public required Usuario Usuario { get; set; }

        public int PerfilId { get; set; }
        public required Perfil Perfil { get; set; }
    }
}