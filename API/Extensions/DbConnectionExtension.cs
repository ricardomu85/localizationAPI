using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;
public static class DbConnectionExtension{
    public static IServiceCollection AddDbConnectionExtension(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment = false)
    {
        var log = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        string connString;
        if ( IsDevelopment)
        {
            connString = configuration.GetConnectionString("ConnectionString")!;
            log.LogInformation($"Connection string development: {connString}");
        }
        else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATABASE_URL")))
        {
            // Use connection string provided at runtime by FlyIO.
            var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL")!;
            log.LogInformation($"Connection string DATABASE_URL: {connUrl}");

            // Parse connection URL to connection string for Npgsql
            connUrl = connUrl.Replace("postgres://", string.Empty);
            var pgUserPass = connUrl.Split("@")[0];
            var pgHostPortDb = connUrl.Split("@")[1];
            var pgHostPort = pgHostPortDb.Split("/")[0];
            var pgDb = pgHostPortDb.Split("/")[1].Split("?")[0];
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
            connString = configuration.GetConnectionString("ConnectionString")!;
            log.LogInformation($"Connection string production : {connString}");
        }

        services.AddDbContext<ApplicationDBContext>(opt =>
        {
            opt.UseNpgsql(connString,
            b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName));
        });
        
        return services;
    }
}