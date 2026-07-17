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
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

// Use CORS before routing/authorization middleware
app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
