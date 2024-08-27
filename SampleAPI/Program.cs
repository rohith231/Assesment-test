using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using log4net.Config;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swagger configuration should be before Build
builder.Services.AddDbContext<SampleApiDbContext>(options =>
    options.UseInMemoryDatabase("SampleDB")); // Configure In-Memory Database
builder.Services.AddTransient<IOrderRepository, OrderRepository>(); // Register your repository

var app = builder.Build(); // Build the application after configuring services
XmlConfigurator.Configure(new System.IO.FileInfo("app.config"));
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
