using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SisInt.Backend.AuthService;
using SisInt.Backend.AuthService.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:80");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Keycloak:Authority"];
    options.Audience = builder.Configuration["Keycloak:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidIssuers = ["http://keycloak:8080/realms/sisint-realm"],
        NameClaimType = "preferred_username",
        RoleClaimType = "realm_access.roles"
    };

    options.RequireHttpsMetadata = false;
    options.MapInboundClaims = false;
});

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
            Thread.Sleep(3000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();