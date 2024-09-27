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

//resolver a depend�ncia do TokenService
builder.Services.AddTransient<TokenService>();
//middleware de autentica��o.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //definir o esquema de autentica��o padr�o: JwtBearer (Bearer Token, JWT Bearer Token, OAuth 1.0 e etc�) 
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //forma que ele vai interrogar a requisi��o para saber onde est� o token: JwtBearer

}).AddJwtBearer(x => //definir como e onde achar esse token, onde est� a chave e como desencriptar esse token.
{
    //dentro desse objeto vamos definir como obter a chave e desemcriptar
    x.TokenValidationParameters = new TokenValidationParameters
    {
        //passando a chave que est� nas configura��es
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        //vamos tirar essas valida��es, pois n�o estamos trabalhando com OIDC.
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


//middleware de autoriza��o.
builder.Services.AddAuthorization(x => {
    //vamos adicionar pol�tica para exigir o role admin
    x.AddPolicy("Admin", p => p.RequireRole("admin"));
});

var app = builder.Build();

//adiciona autentica��o
app.UseAuthentication();
//adiciona autoriza��o
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
app.MapGet("/admin", () => "Voc� tem acesso").RequireAuthorization("admin");

app.Run();
