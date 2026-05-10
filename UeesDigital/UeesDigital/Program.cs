using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UeesDigital.Application.Services;
using UeesDigital.Domain.Interfaces;
using UeesDigital.Infrastructure.Identity;
using UeesDigital.Infrastructure.Persistence;
using UeesDigital.Infrastructure.Persistence.Repositories;
using UeesDigital.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=UeesDigitalDb;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentityCore<AppIdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IFechaDisponibleRepository, FechaDisponibleRepository>();
builder.Services.AddScoped<IHorarioDisponibleRepository, HorarioDisponibleRepository>();
builder.Services.AddScoped<ITramiteRepository, TramiteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<FechaDisponibleService>();
builder.Services.AddScoped<HorarioDisponibleService>();
builder.Services.AddScoped<TramiteService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
