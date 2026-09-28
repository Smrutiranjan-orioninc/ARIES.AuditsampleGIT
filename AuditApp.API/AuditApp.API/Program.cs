using Microsoft.EntityFrameworkCore;
using AuditApp.API.Data;
using AuditApp.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpContextAccessor for telemetry
builder.Services.AddHttpContextAccessor();

// Add Application Insights (primary – uses APPLICATIONINSIGHTS_CONNECTION_STRING)
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
});

// Fan-out: forward all API telemetry to the remaining four App Insights instances
builder.Services.AddApplicationInsightsTelemetryProcessor<AuditApp.API.Middleware.MulticastTelemetryProcessor>();

// Add custom TelemetryInitializer to set CloudRoleName based on module
builder.Services.AddSingleton<Microsoft.ApplicationInsights.Extensibility.ITelemetryInitializer, AuditApp.API.Middleware.ModuleTelemetryInitializer>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuditApp.API.Middleware.ExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Test log for AppInsights
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("AuditApp API Started and AppInsights initialized.");

app.Run();
