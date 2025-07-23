namespace SisInt.Backend.AuthService.Models
{
    public class PermissaoPerfil
    {
        public int PermissaoId { get; set; }
        public required Permissao Permissao { get; set; }

        public int PerfilId { get; set; }
        public required Perfil Perfil { get; set; }
    }
}