using Infrastructure.Database;
using Infrastructure.DI;
using Microsoft.EntityFrameworkCore;
using Web;
using Web.ApiServices.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddConfiguredCors();
builder.Services.AddControllers();

builder.Services
    .AddConfiguredAuthorization()
    .AddConfiguredServices();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<TelegramBotConfig>(builder.Configuration.GetSection("TelegramBotConfig"));

var app = builder.Build();

await app.PrepareAppAsync();

app.Run();