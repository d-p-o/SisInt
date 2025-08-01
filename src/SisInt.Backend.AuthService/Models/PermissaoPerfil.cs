namespace SisInt.Backend.AuthService.Models
{
    /// <summary>
    /// Entidade de junção para o relacionamento muitos-para-muitos entre Permissao e Perfil.
    /// </summary>
    public class PermissaoPerfil
    {
        public int PermissaoId { get; set; }
        public required Permissao Permissao { get; set; }

        public int PerfilId { get; set; }
        public required Perfil Perfil { get; set; }
    }
}