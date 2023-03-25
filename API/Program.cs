using AutoMapper;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Mapper;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
  {
      options.AddPolicy(name: "_myAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:5001",
                                              "http://localhost:5000");
                      });
  });



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var log = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
string connString;
if (builder.Environment.IsDevelopment())
{
    connString = builder.Configuration.GetConnectionString("ConnectionString")!;
    log.LogInformation($"Connection string development: {connString}");
}
else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")))
{
    // Use connection string provided at runtime by FlyIO.
    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    log.LogInformation($"Connection string DATABASE_URL: {connUrl}");

    // Parse connection URL to connection string for Npgsql
    connUrl = connUrl.Replace("postgres://", string.Empty);
    var pgUserPass = connUrl.Split("@")[0];
    var pgHostPortDb = connUrl.Split("@")[1];
    var pgHostPort = pgHostPortDb.Split("/")[0];
    var pgDb = pgHostPortDb.Split("/")[1];
    var pgUser = pgUserPass.Split(":")[0];
    var pgPass = pgUserPass.Split(":")[1];
    var pgHost = pgHostPort.Split(":")[0];
    var pgPort = pgHostPort.Split(":")[1];
    var updatedHost = pgHost.Replace("flycast", "internal");

    connString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
    log.LogInformation($"Connection string production: {connString}");



}
else
{
    log.LogInformation($"DATABASE_URL is null or empty");
    connString = builder.Configuration.GetConnectionString("ConnectionString")!;
    log.LogInformation($"Connection string production : {connString}");
}

builder.Services.AddDbContext<ApplicationDBContext>(opt =>
{
    opt.UseNpgsql(connString,
    b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName));
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

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(policy =>
               policy
                   .WithOrigins(
                                   "https://localhost:5001",
                                   "http://localhost:5000")
                   .AllowAnyMethod()
                   .WithHeaders(HeaderNames.ContentType));

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
        logger.LogError(ex, "Error en migraci�n");
    }
}
app.Run();
