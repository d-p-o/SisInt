using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using SisInt.Backend.AuthService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configura o Kestrel para ouvir na porta 80, ideal para o ambiente Docker.
builder.WebHost.UseUrls("http://*:80");

// Adiciona os serviços ao contêiner de Injeção de Dependência.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    // Configura a serialização JSON para lidar com referências circulares de objetos.
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configuração do Swagger/OpenAPI para documentação da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SisInt.Backend.AuthService", Version = "v1" });

    // Configura o Swagger para aceitar autenticação via JWT Bearer.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração de autenticação JWT com Keycloak.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];
    options.RequireHttpsMetadata = false; // Desabilitado para ambiente de desenvolvimento.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuers = [builder.Configuration["Keycloak:Authority"]!],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Keycloak:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
    };
    options.MapInboundClaims = false;
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                var realmAccessClaim = context.Principal.FindFirst("realm_access");

                if (realmAccessClaim != null)
                {
                    try
                    {
                        using var realmAccessValue = JsonDocument.Parse(realmAccessClaim.Value);
                        if (realmAccessValue.RootElement.TryGetProperty("roles", out var rolesElement))
                        {
                            foreach (var role in rolesElement.EnumerateArray())
                            {
                                if (role.GetString() is string roleString)
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Role, roleString));
                                }
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        Console.WriteLine("Could not parse 'realm_access' claim as JSON.");
                    }
                }
            }
            return Task.CompletedTask;
        }
    };
});

// Configuração de políticas de autorização baseadas em roles.
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));

var app = builder.Build();

// Bloco de código para aplicar migrações no startup.
// A aplicação tentará aplicar as migrações e, se falhar, tentará novamente
// com um atraso, o que é útil quando o contêiner do SQL Server ainda está iniciando.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var maxRetries = 10;
    var currentRetry = 0;

    while (currentRetry < maxRetries)
    {
        try
        {
            Console.WriteLine("Tentando aplicar as migrações...");
            dbContext.Database.Migrate();
            Console.WriteLine("Migrações aplicadas com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            currentRetry++;
            Console.WriteLine($"Erro ao aplicar migrações. Tentativa {currentRetry}/{maxRetries}. Erro: {ex.Message}");
            Thread.Sleep(3000); // Aguarda 3 segundos antes de tentar novamente.
        }
    }
}

// Configura o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();