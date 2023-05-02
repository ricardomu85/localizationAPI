using AutoMapper;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Mapper;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.Extensions;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimitServices(builder.Configuration);
builder.Services.AddDbConnectionExtension(builder.Configuration, builder.Environment.IsDevelopment());

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

app.AddWebApplicationExtension();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var loggerFactory = service.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<Program>();

    try
    {
        var context = service.GetRequiredService<ApplicationDBContext>();
        var tablePais = "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Paises')";
        // Verificar si la tabla existe
        var tableExists = context.Database.ExecuteSqlRaw(tablePais);
        logger.LogInformation($"Script : {tablePais}");
        if (tableExists == 1)
        {
            // La tabla existe, no es necesario migrar
            logger.LogInformation("La tabla existe!");
        }
        else
        {
            // La tabla no existe, es necesario migrar
            await context.Database.MigrateAsync();
            logger.LogInformation("La tabla no existe y ha sido creada!");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error en migración");
    }
}
app.Run();
