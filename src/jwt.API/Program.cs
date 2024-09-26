using asp.net_jwt.Services;

var builder = WebApplication.CreateBuilder(args);

//resolver a dependência do TokenService
builder.Services.AddTransient<TokenService>();

var app = builder.Build();

app.MapGet("/", (TokenService service) => service.Create());

app.Run();
