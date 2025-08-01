using Microsoft.EntityFrameworkCore;
using SisInt.Backend.AuthService.Models;

namespace SisInt.Backend.AuthService.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // DbSets para mapear entidades para tabelas no banco de dados.
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; }
        public DbSet<PermissaoPerfil> PermissaoPerfis { get; set; }
        public DbSet<LogAcesso> LogAcessos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações para tabelas de junção com chaves compostas.
            modelBuilder.Entity<UsuarioPerfil>().HasKey(up => new { up.UsuarioId, up.PerfilId });
            modelBuilder.Entity<PermissaoPerfil>().HasKey(pp => new { pp.PermissaoId, pp.PerfilId });

            // Configurações de relacionamento muitos-para-muitos para Usuario-Perfil.
            modelBuilder.Entity<UsuarioPerfil>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.UsuarioPerfis)
                .HasForeignKey(up => up.UsuarioId);

            modelBuilder.Entity<UsuarioPerfil>()
                .HasOne(up => up.Perfil)
                .WithMany(p => p.UsuarioPerfis)
                .HasForeignKey(up => up.PerfilId);

            // Configurações de relacionamento muitos-para-muitos para Permissao-Perfil.
            modelBuilder.Entity<PermissaoPerfil>()
                .HasOne(pp => pp.Permissao)
                .WithMany(p => p.PermissaoPerfis)
                .HasForeignKey(pp => pp.PermissaoId);

            modelBuilder.Entity<PermissaoPerfil>()
                .HasOne(pp => pp.Perfil)
                .WithMany(p => p.PermissaoPerfis)
                .HasForeignKey(pp => pp.PerfilId);

            base.OnModelCreating(modelBuilder);
        }
    }
}