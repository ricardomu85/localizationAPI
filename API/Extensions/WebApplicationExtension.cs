using AspNetCoreRateLimit;

namespace API.Extensions;

public static class WebApplicationExtension
{
  public static WebApplication AddWebApplicationExtension(this WebApplication app)
  {
    app.UseRouting();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }
    app.UseIpRateLimiting();
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });

    return app;
  }
}