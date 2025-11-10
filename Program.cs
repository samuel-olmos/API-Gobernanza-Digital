using API_Gobernanza_Digital.Interfaces;
using API_Gobernanza_Digital.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<GobernanzaDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Registra el HttpClient (NECESARIO para llamar a otras APIs)
builder.Services.AddHttpClient();

// 2. Registra tu IBoletaService (que ya tenías)
builder.Services.AddScoped<IBoletaService, BoletaService>(); 

// 3. Registra el nuevo IPagoService
builder.Services.AddScoped<IPagoService, PagoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
