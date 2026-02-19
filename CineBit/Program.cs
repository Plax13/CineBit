using CineBit.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurazione DbContext
builder.Services.AddDbContext<CinebitDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Registrazione repository generico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Aggiungi controller
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
