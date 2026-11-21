using InsuranceRiskApi.Application.Interfaces;
using InsuranceRiskApi.Application.Services;
using InsuranceRiskApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Insurance Risk & Premium Calculator API",
        Version = "v1",
        Description = "Computes a risk score and suggested premium for an applicant, and persists the resulting quote."
    });
});

// EF Core (SQLite - zero setup)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=insurance.db"));

// App services
builder.Services.AddScoped<IRiskEngine, RiskEngine>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

var app = builder.Build();

// Ensure DB + schema exist on startup (fine for a demo project; use migrations in real life)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance Risk API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory in integration tests
public partial class Program { }
