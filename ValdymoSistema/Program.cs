using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ValdymoSistema.Controllers;
using ValdymoSistema.Data;

namespace ValdymoSistema
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var serviceProvider = services.GetRequiredService<IServiceProvider>();
                    var config = services.GetRequiredService<IConfiguration>();
                    var database = services.GetRequiredService<ApplicationDbContext>();
                    database.Database.Migrate();
                    DatabaseSeeder.SeedData(serviceProvider, config).Wait();
                    DatabaseSeeder.CreateRoles(serviceProvider, config).Wait();
                    DatabaseSeeder.SeedLightEvents(serviceProvider).Wait();
                }
                catch (Exception exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Error occurred while seeding data");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
