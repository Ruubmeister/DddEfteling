using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Boundary;
using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundary;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Rides
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                services.GetRequiredService<IEmployeeClient>();
                services.GetRequiredService<IEventProducer>();
                services.GetRequiredService<IVisitorClient>();
                IRideControl rideControl = services.GetRequiredService<IRideControl>();
                IEventConsumer eventConsumer = services.GetRequiredService<IEventConsumer>();

                _ = Task.Run(() => eventConsumer.Listen());

                _ = Task.Run(() =>
                {
                    while (true)
                    {
                        rideControl.HandleOpenRides();
                        Task.Delay(1000).Wait();
                    }
                });

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
