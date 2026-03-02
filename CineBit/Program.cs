using CineBit.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurazione DbContext
builder.Services.AddDbContext<CinebitDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Registrazione repository generico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registrazione HttpClientFactory
builder.Services.AddHttpClient();

// Aggiungi controller
builder.Services.AddControllers();

// Aggiungi i servizi per la dependency injection (gestione ottimizzata delle chiamate HTTP)
builder.Services.AddHttpClient<AiService>();
builder.Services.AddHttpClient<TmdbService>();


// --- Aggiungi CORS (Cross-Origin Resource Sharing) per far comunicare le porte 5000 <-> 4200 ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- abilita CORS per comunicazion porta 4200 <-> 5000 ---
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();