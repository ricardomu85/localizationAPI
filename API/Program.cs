using AutoMapper;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connString;
connString = builder.Configuration.GetConnectionString("ConnectionString")!;

builder.Services.AddDbContext<ApplicationDBContext>(opt =>
{
    opt.UseNpgsql(connString, b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName));

});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var loggerFactory = service.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<Program>();

    try
    {
        var context = service.GetRequiredService<ApplicationDBContext>();

        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error en migración");
    }
}
app.Run();
