using asp.net_jwt;
using asp.net_jwt.Extentions;
using asp.net_jwt.Models;
using asp.net_jwt.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//resolver a dependência do TokenService
builder.Services.AddTransient<TokenService>();
//middleware de autenticação.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //definir o esquema de autenticação padrão: JwtBearer (Bearer Token, JWT Bearer Token, OAuth 1.0 e etc…) 
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //forma que ele vai interrogar a requisição para saber onde está o token: JwtBearer

}).AddJwtBearer(x => //definir como e onde achar esse token, onde está a chave e como desencriptar esse token.
{
    //dentro desse objeto vamos definir como obter a chave e desemcriptar
    x.TokenValidationParameters = new TokenValidationParameters
    {
        //passando a chave que está nas configurações
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        //vamos tirar essas validações, pois não estamos trabalhando com OIDC.
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


//middleware de autorização.
builder.Services.AddAuthorization(x => {
    //vamos adicionar política para exigir o role admin
    x.AddPolicy("Admin", p => p.RequireRole("admin"));
});

var app = builder.Build();

//adiciona autenticação
app.UseAuthentication();
//adiciona autorização
app.UseAuthorization();

app.MapGet("/login", (TokenService service) =>
{
    //Criar um novo e passar
    var user = new User("Marx Teixeira", 1, "email@marx.com", "https://balta.io", "sdfj", new[] { "student", "premium" });

    return service.Create(user);
});

app.MapGet("/restrito", (ClaimsPrincipal user) => new
{
    id = user.Id(),
    name = user.Name(),
    email = user.Email(),
    givenName = user.GivenName(),
    image = user.Image()

}).RequireAuthorization();

//para testes o role admin
app.MapGet("/admin", () => "Você tem acesso").RequireAuthorization("admin");

app.Run();
