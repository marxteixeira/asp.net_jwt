using asp.net_jwt.Models;
using asp.net_jwt.Services;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

//resolver a depend�ncia do TokenService
builder.Services.AddTransient<TokenService>();
//middleware de autentica��o.
builder.Services.AddAuthentication();
//middleware de autoriza��o.
builder.Services.AddAuthorization();

var app = builder.Build();

//adiciona autentica��o
app.UseAuthentication();
//adiciona autoriza��o
app.UseAuthorization();

app.MapGet("/", (TokenService service) =>
{
    //Criar um novo e passar
    var user = new User("Marx Teixeira", 1, "email@marx.com", "https://balta.io", "sdfj", new[] { "student", "premium" });

    return service.Create(user);
});

app.Run();
