using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace SisInt.Backend.AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestAuthController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetAuthenticatedData()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            return Ok($"Olá, {userName ?? "Usuário Anônimo"}! " +
                      $"Seu ID de usuário é: {userId ?? "Não disponível"}. " +
                      $"Seu email é: {userEmail ?? "Não disponível"}. " +
                      $"Você está autenticado com sucesso!");
        }

        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            return Ok("Este é um endpoint público, não requer autenticação.");
        }
    }
}