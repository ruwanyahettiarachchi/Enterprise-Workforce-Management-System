using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Enterprise.Application;
using Enterprise.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Clean Architecture Layers dependency registrations
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. Add Controller support
builder.Services.AddControllers();

// 3. Add API Explorer & Swagger for visual validation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Enterprise Workforce Management System (EWMS) API",
        Version = "v1",
        Description = "An enterprise-grade Clean Architecture Web API to manage workforce entities, departments, and metric dashboards.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Enterprise Development Team",
            Email = "support@ewms.com"
        }
    });

    // Enable XML Comments in Swagger UI
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// 4. Configure CORS for local Angular development
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Run database migrations and seed default data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<Enterprise.Infrastructure.EnterpriseDbContext>();
        await Enterprise.Infrastructure.DbSeeder.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseHttpsRedirection();

// Use CORS before routing/authorization middleware
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
