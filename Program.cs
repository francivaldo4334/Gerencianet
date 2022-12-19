using OfficesafeAndGerencianet.Dtos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var Configuration = builder.Configuration;
var services = builder.Services;
//Credentials Gerencianet
GerencianetCredenciais credGerencianet = new GerencianetCredenciais();
credGerencianet.ClientId = Configuration.GetSection("Gerencianet").GetSection("ClientId").Value;
credGerencianet.ClientSecret = Configuration.GetSection("Gerencianet").GetSection("ClientSecret").Value;
credGerencianet.CrtPath = Configuration.GetSection("Gerencianet").GetSection("CrtPath").Value;
credGerencianet.RouteHttp = Configuration.GetSection("Gerencianet").GetSection("RouteHttp").Value;
credGerencianet.Sandbox = bool.Parse(Configuration.GetSection("Gerencianet").GetSection("Sandbox").Value);
credGerencianet.RouteAuth = Configuration.GetSection("Gerencianet").GetSection("RouteAuth").Value;
services.AddSingleton(credGerencianet);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
