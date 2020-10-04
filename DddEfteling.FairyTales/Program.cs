using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Boundary;
using DddEfteling.FairyTales.Controls;
using DddEfteling.Shared.Boundary;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DddEfteling.FairyTales
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                services.GetRequiredService<IEventProducer>();
                services.GetRequiredService<IFairyTaleControl>();
                
                IEventConsumer eventConsumer = services.GetRequiredService<IEventConsumer>();
                _ = Task.Run(() => eventConsumer.Listen());
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}