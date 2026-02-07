using Microsoft.EntityFrameworkCore;
using pBiblioteca.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// Banco (PostgreSQL) - precisa existir PostgresContext
builder.Services.AddDbContext<PostgresContext>();

// DI - precisam existir essas interfaces/classes
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
