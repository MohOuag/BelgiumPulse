using BelgiumPulse.Api.Middleware;
using BelgiumPulse.Application;
using BelgiumPulse.Infrastructure;
using BelgiumPulse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<BelgiumPulseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key manquant dans la config");

//Lecture du header Authorization sur chaque requete
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "database",
        tags: new[] { "db", "sql" })
    .AddRedis(
        builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379",
        name: "redis");

builder.Services.AddCors(options =>
{
    options.AddPolicy("BelgiumPulsePolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "BelgiumPulse API");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("BelgiumPulsePolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            entries = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration
            })
        };

        await context.Response.WriteAsync(
            System.Text.Json.JsonSerializer.Serialize(result));
    }
});

// Migration automatique au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<BelgiumPulseDbContext>();
    await db.Database.MigrateAsync();
}

// Minimal API — exemple de ping simple
app.MapGet("/ping", () => Results.Ok(new
{
    status = "alive",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}))
.WithTags("Health")
.WithOpenApi();

app.Run();
